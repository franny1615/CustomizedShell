using CommunityToolkit.Mvvm.Messaging;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace CustomizedShell.Pages;

public class LoginPage : BasePage
{
	#region Private Variables
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
    public LoginPage(LoginViewModel loginViewModel)
	{
        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);

        BindingContext = loginViewModel;

		_Username.Placeholder = Lang["Username"];
		_Password.Placeholder = Lang["Password"];
		_Login.Text = Lang["Login"];
		_Register.Text = Lang["Register"];

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
            Text = Lang["WelcomeTo"],
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = 8
        });
        _ContentLayout.Add(new Label
        {
            Text = Lang["WelcomeBack"],
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
			Text = Lang["Or"],
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
    }
    protected override void OnDisappearing()
    {
        _Login.Clicked -= Login;
        _Register.Clicked -= Register;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Register(object sender, ClickedEventArgs e)
    {
        await this.Navigation.PushAsync(new RegisterPage(_LoginViewModel));
    }

    private async void Login(object sender, ClickedEventArgs e)
    {
        if (string.IsNullOrEmpty(_Username.Text) ||
            string.IsNullOrEmpty(_Password.Text))
        {
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
                Lang["Login"], 
                Lang["InvalidLogin"],
                Lang["TryAgain"]);
        }

        _Username.Text = "";
        _Password.Text = "";
    }
    #endregion
}