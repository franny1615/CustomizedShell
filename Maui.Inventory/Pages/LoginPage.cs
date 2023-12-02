using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.Inventory.Pages;

public class LoginPage : BasePage
{
    #region Private Variables
    private readonly ILanguageService _LanguageService;
    private LoginViewModel _LoginViewModel => (LoginViewModel) BindingContext;
	private readonly ScrollView _ContentScroll = new();
	private readonly VerticalStackLayout _ContentLayout = new()
	{
		Padding = 16,
		VerticalOptions = LayoutOptions.Center,
	};
	private readonly StyledEntry _Username = new();
	private readonly StyledEntry _Password = new() { IsPassword = true };
	private readonly FloatingActionButton _Login = new()
	{
		TextColor = Colors.White,
		FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
		FABStyle = FloatingActionButtonStyle.Extended,
	};
	private readonly FloatingActionButton _Register = new()
	{
        TextColor = Colors.White,
        FABBackgroundColor = Application.Current.Resources["PrimaryShade"] as Color,
        FABStyle = FloatingActionButtonStyle.Extended
	};
    #endregion

    #region Constructor
    public LoginPage(
        ILanguageService languageService, 
        LoginViewModel loginViewModel) : base(languageService)
	{
        _LanguageService = languageService;

        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);

        BindingContext = loginViewModel;

		_Username.Placeholder = languageService.StringForKey("Username");
		_Password.Placeholder = languageService.StringForKey("Password");
		_Login.Text = languageService.StringForKey("Login");
		_Register.Text = languageService.StringForKey("Register");

		_ContentLayout.Add(new Border
		{
			Stroke = Colors.Transparent,
			StrokeShape = new RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Application.Current.Resources["Primary"] as Color,
			HorizontalOptions = LayoutOptions.Center,
            Padding = 0,
			Margin = 0,
			Content = new Image
			{
				WidthRequest = 124,
				HeightRequest = 124,
				Source = "app_ic.png",
				HorizontalOptions = LayoutOptions.Center,
			}
		});
        _ContentLayout.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 16
        });
        _ContentLayout.Add(new Label
		{
            Text = languageService.StringForKey("WelcomeTo"),
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = 8
        });
        _ContentLayout.Add(new Label
        {
            Text = languageService.StringForKey("WelcomeBack"),
            FontSize = 16,
            FontAttributes = FontAttributes.None,
            HorizontalOptions = LayoutOptions.Center,
            Margin = 0
        });
        _ContentLayout.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 16
        });
		_ContentLayout.Add(_Username);
        _ContentLayout.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 8
        });
        _ContentLayout.Add(_Password);
		_ContentLayout.Add(new BoxView
		{
			Color = Colors.Transparent,
			HeightRequest = 64
		});
		_ContentLayout.Add(_Login);
		_ContentLayout.Add(new Label
		{
			Text = languageService.StringForKey("Or"),
			FontSize = 16,
			FontAttributes = FontAttributes.Bold,
			HorizontalOptions = LayoutOptions.Center,
			Margin = 8
		});
		_ContentLayout.Add(_Register);

		_ContentScroll.Content = _ContentLayout;

		Content = _ContentScroll;
	}
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Login.Clicked += Login;
        _Register.Clicked += Register;
        _Username.TextChanged += UsernameChanged;
        _Password.TextChanged += PasswordChanged;
    }
    protected override void OnDisappearing()
    {
        _Login.Clicked -= Login;
        _Register.Clicked -= Register;
        _Username.TextChanged -= UsernameChanged;
        _Password.TextChanged -= PasswordChanged;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private void UsernameChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length > 0)
        {
            _Username.StatusColor = Colors.Green;
            _Username.StatusIcon = null;
            _Username.StatusText = "";
        }
        else
        {
            _Username.StatusColor = Colors.Black;
            _Username.StatusIcon = null;
            _Username.StatusText = "";
        }
    }

    private void PasswordChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length > 0)
        {
            _Password.StatusColor = Colors.Green;
            _Password.StatusIcon = null;
            _Password.StatusText = "";
        }
        else
        {
            _Password.StatusColor = Colors.Black;
            _Password.StatusIcon = null;
            _Password.StatusText = "";
        }
    }

    private async void Register(object sender, ClickedEventArgs e)
    {
        await this.Navigation.PushAsync(new RegisterPage(_LanguageService, _LoginViewModel));
    }

    private async void Login(object sender, ClickedEventArgs e)
    {
        if (string.IsNullOrEmpty(_Username.Text) ||
            string.IsNullOrEmpty(_Password.Text))
        {
            _Username.StatusColor = Colors.Red;
            _Username.StatusIcon = "info.png";
            _Username.StatusText = _LanguageService.StringForKey("NotEmpty");

            _Password.StatusColor = Colors.Red;
            _Password.StatusIcon = "info.png";
            _Password.StatusText = _LanguageService.StringForKey("NotEmpty");
            return;
        }

        bool validLogin = await _LoginViewModel.Login(
            _Username.Text,
            _Password.Text
        );

        if (validLogin)
        {
            WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-in"));
        }
        else
        {
            await this.DisplayAlert(
                _LanguageService.StringForKey("Login"), 
                _LanguageService.StringForKey("InvalidLogin"),
                _LanguageService.StringForKey("TryAgain"));
        }

        _Username.Text = "";
        _Password.Text = "";
    }
    #endregion
}