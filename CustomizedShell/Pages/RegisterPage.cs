using CommunityToolkit.Mvvm.Messaging;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Microsoft.Maui.Controls.Shapes;

namespace CustomizedShell;

public class RegisterPage : BasePage
{
    #region Private Propertires
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
    private readonly StyledEntry _Email = new() { Keyboard = Keyboard.Email };
    private readonly FloatingActionButton _Register = new()
	{
        TextColor = Colors.White,
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        FABStyle = FloatingActionButtonStyle.Extended
	};
    private readonly FloatingActionButton _GoBack = new()
	{
        TextColor = Colors.White,
        FABBackgroundColor = Application.Current.Resources["PrimaryShade"] as Color,
        FABStyle = FloatingActionButtonStyle.Extended
	};
    #endregion

    #region Constructor
    public RegisterPage(
        ILanguageService languageService, 
        LoginViewModel loginViewModel) : base(languageService)
    {
        _LanguageService = languageService;

        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);

        BindingContext = loginViewModel;

        _Username.Placeholder = _LanguageService.StringForKey("Username");
		_Password.Placeholder = _LanguageService.StringForKey("Password");
        _Email.Placeholder = _LanguageService.StringForKey("Email");
        _Register.Text = _LanguageService.StringForKey("Register");
        _GoBack.Text = _LanguageService.StringForKey("GoBack");

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
            Text = _LanguageService.StringForKey("Register"),
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = 8
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
            HeightRequest = 8
        });
        _ContentLayout.Add(_Email);
		_ContentLayout.Add(new BoxView
		{
			Color = Colors.Transparent,
			HeightRequest = 64
		});
        _ContentLayout.Add(_Register);
        _ContentLayout.Add(new Label
		{
			Text = _LanguageService.StringForKey("Or"),
			FontSize = 16,
			FontAttributes = FontAttributes.Bold,
			HorizontalOptions = LayoutOptions.Center,
			Margin = 8
		});
        _ContentLayout.Add(_GoBack);

        _ContentScroll.Content = _ContentLayout;
        Content = _ContentScroll;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Register.Clicked += Register;
        _GoBack.Clicked += GoBack;
    }

    protected override void OnDisappearing()
    {
        _Register.Clicked -= Register;
        _GoBack.Clicked -= GoBack;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Register(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_Username.Text))
        {
            await this.DisplayAlert(
                _LanguageService.StringForKey("Register"),
                _LanguageService.StringForKey("UsernameRequired"),
                _LanguageService.StringForKey("Ok")
            );
            return;
        }

        if (string.IsNullOrEmpty(_Password.Text))
        {
            await this.DisplayAlert(
                _LanguageService.StringForKey("Register"),
                _LanguageService.StringForKey("PasswordRequired"),
                _LanguageService.StringForKey("Ok")
            );
            return;
        }

        if (string.IsNullOrEmpty(_Email.Text))
        {
            await this.DisplayAlert(
                _LanguageService.StringForKey("Register"),
                _LanguageService.StringForKey("EmailRequired"),
                _LanguageService.StringForKey("Ok")
            );
            return;
        }

        await _LoginViewModel.RegisterUser(
            _Username.Text,
            _Password.Text,
            _Email.Text
        );
        WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-in"));
    }

    private async void GoBack(object sender, EventArgs e)
    {
        await this.Navigation.PopAsync();
    }
    #endregion
}
