using Maui.Components.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace CustomizedShell.Pages;

public class LoginPage : BasePage
{
	#region Private Variables
	private readonly ScrollView _ContentScroll = new();
	private readonly VerticalStackLayout _ContentLayout = new()
	{
		Padding = 16,
		VerticalOptions = LayoutOptions.Center,
	};
	private readonly StyledEntry _Username = new();
	private readonly StyledEntry _Password = new();
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
    public LoginPage()
	{
		HideNavBar();

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

		Page.Content = _ContentScroll;
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
    private void Register(object sender, ClickedEventArgs e)
    {
        // TODO: take them to registration page, by pushing onto navigation stack
    }

    private void Login(object sender, ClickedEventArgs e)
    {
        // TODO: validate in viewmodel if what they entered matched some user inside the db
    }
    #endregion
}