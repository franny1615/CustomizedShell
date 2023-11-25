using CommunityToolkit.Mvvm.Messaging;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
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
        _Username.TextChanged += UsernameChanged;
        _Password.TextChanged += PasswordChanged;
        _Email.TextChanged += EmailChanged;
    }

    protected override void OnDisappearing()
    {
        _Register.Clicked -= Register;
        _GoBack.Clicked -= GoBack;
        _Username.TextChanged -= UsernameChanged;
        _Password.TextChanged -= PasswordChanged;
        _Email.TextChanged -= EmailChanged;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Register(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_Username.Text))
        {
            _Username.StatusColor = Colors.Red;
            _Username.StatusIcon = "info.png";
            _Username.StatusText = _LanguageService.StringForKey("UsernameRequired");
        }

        if (string.IsNullOrEmpty(_Password.Text))
        {
            _Password.StatusColor = Colors.Red;
            _Password.StatusIcon = "info.png";
            _Password.StatusText = _LanguageService.StringForKey("PasswordRequired");
        }

        if (string.IsNullOrEmpty(_Email.Text))
        {
            _Email.StatusColor = Colors.Red;
            _Email.StatusIcon = "info.png";
            _Email.StatusText = _LanguageService.StringForKey("EmailRequired");
        }

        if (string.IsNullOrEmpty(_Email.Text) || 
            string.IsNullOrEmpty(_Password.Text) || 
            string.IsNullOrEmpty(_Username.Text) ||
            !StringUtils.IsValidEmail(_Email.Text))
        {
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

    private void EmailChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length > 0)
        {
            if (StringUtils.IsValidEmail(e.NewTextValue))
            {
                _Email.StatusColor = Colors.Green;
                _Email.StatusIcon = null;
                _Email.StatusText = "";
            }
            else
            {
                _Email.StatusColor = Colors.Red;
                _Email.StatusIcon = "info.png";
                _Email.StatusText = _LanguageService.StringForKey("EmailInvalid");
            }
        }
        else
        {
            _Email.StatusColor = Colors.Black;
            _Email.StatusIcon = null;
            _Email.StatusText = "";
        }
    }
    #endregion
}
