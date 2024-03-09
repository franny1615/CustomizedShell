using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.Pages.AdminPages;
using Maui.Inventory.Pages.UserPages;
using Maui.Inventory.ViewModels.AdminVM;
using Maui.Inventory.ViewModels.UserVM;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages;

public class LandingPage : BasePage
{
	#region Private Properties
	private readonly ILanguageService _LangService;
	private readonly AdminRegisterViewModel _AdminVM;
	private readonly AdminLoginViewModel _AdminLoginVM;
	private readonly UserLoginViewModel _UserLoginVM;
	private readonly VerticalStackLayout _ProductContainer = new()
	{
		Spacing = 16,
		Children = 
		{
			new Border
			{
				HorizontalOptions = LayoutOptions.Start,
				Padding = 0,
				Margin = 0,
				BackgroundColor = Application.Current.Resources["Primary"] as Color,
				Stroke = Colors.Transparent,
				StrokeShape = new RoundRectangle { CornerRadius = 12 },
				Content = new Image
				{
					WidthRequest = 72,
					HeightRequest = 72,
					Source = "app_ic.png"
				}
			},
		}
	};
	private readonly Label _ProductName = new()
	{
		FontSize = 21,
		FontAttributes = FontAttributes.Bold,
		HorizontalOptions = LayoutOptions.Start
	};
	private readonly Grid _ContentLayout = new()
	{
		Padding = new Thickness(16, 32, 16, 16),
		RowSpacing = 0,
		RowDefinitions = Rows.Define(Star, Star)
	};
	private readonly VerticalStackLayout _ActionsContainer = new()
	{
		Spacing = 12
	};
	private readonly Label _AlreadyRegistered = new()
	{
		FontSize = 16,
		HorizontalOptions = LayoutOptions.Start
	};
	private readonly FloatingActionButton _EmployeeLogin = new()
	{
		FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
		TextColor = Colors.White,
		FABStyle = FloatingActionButtonStyle.Extended
	};
	private readonly FloatingActionButton _EmployerLogin = new()
	{
		FABBackgroundColor = Application.Current.Resources["PrimaryShade"] as Color,
		TextColor = Colors.White,
		FABStyle = FloatingActionButtonStyle.Extended
	};
	private readonly FloatingActionButton _Register = new()
	{
		FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
		TextColor = Colors.White,
		FABStyle = FloatingActionButtonStyle.Extended
	};
	private readonly HorizontalRule _OR = new();
    private readonly MaterialPicker _LanguagePicker = new()
    {
        Icon = MaterialIcon.Language,
        IconColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Application.Current.Resources["TextColor"] as Color,
        ItemsSource = new List<string>
        {
            "English",
            "Español"
        }
    };
    #endregion

    #region Constructor
    public LandingPage(
		ILanguageService languageService,
		AdminRegisterViewModel adminVM,
		AdminLoginViewModel adminLoginVM,
		UserLoginViewModel userLoginVM) : base(languageService)
	{
		Shell.SetNavBarIsVisible(this, false);
		Shell.SetTabBarIsVisible(this, false);
		NavigationPage.SetHasNavigationBar(this, false);

		_LangService = languageService;
		_AdminVM = adminVM;
		_AdminLoginVM = adminLoginVM;
		_UserLoginVM = userLoginVM;

		_ProductContainer.Add(_ProductName);
		_ProductName.Text = LanguageService.StringForKey("Product");

		_AlreadyRegistered.Text = LanguageService.StringForKey("AlreadyRegistered");
		_EmployeeLogin.Text = LanguageService.StringForKey("EmployeeLogin");
		_EmployerLogin.Text = LanguageService.StringForKey("AdminLogin");
		_Register.Text = LanguageService.StringForKey("Register");
		_OR.Text = LanguageService.StringForKey("OR");

		_ActionsContainer.Add(_AlreadyRegistered);
		_ActionsContainer.Add(_EmployeeLogin);
		_ActionsContainer.Add(_EmployerLogin);
		_ActionsContainer.Add(_OR.Margin(new Thickness(8, 12, 8, 12)));
		_ActionsContainer.Add(_Register);

		_ContentLayout.Add(_ProductContainer.Row(0));
		_ContentLayout.Add(_LanguagePicker.Row(0).End().Top());
		_ContentLayout.Add(_ActionsContainer.Row(1).Bottom());

		Content = _ContentLayout;
        
		_EmployeeLogin.Clicked += EmployeeLogin;
        _EmployerLogin.Clicked += EmployerLogin;
        _Register.Clicked += EmployerRegister;
        _LanguagePicker.PickedItem += LanguageChanged;

        WeakReferenceMessenger.Default.Register<InternalMessage>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => ProcessInternalMsg(msg.Value.ToString()));
        });
    }
    ~LandingPage()
	{
        WeakReferenceMessenger.Default.Unregister<InternalMessage>(this);
        _EmployeeLogin.Clicked -= EmployeeLogin;
        _EmployerLogin.Clicked -= EmployerLogin;
        _Register.Clicked -= EmployerRegister;
		_LanguagePicker.PickedItem -= LanguageChanged;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();

        string lang = Preferences.Get(Constants.Language, "English");
        _LanguagePicker.SelectedItem = lang;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

	#region Helpers
	private void EmployeeLogin(object sender, EventArgs e)
	{
		_UserLoginVM.Clear();
		Navigation.PushAsync(new UserLoginPage(_LangService, _UserLoginVM));
	}

	private void EmployerLogin(object sender, EventArgs e)
	{
		_AdminLoginVM.Clear();
		Navigation.PushAsync(new AdminLoginPage(_LangService, _AdminLoginVM));
	}

	private void EmployerRegister(object sender, EventArgs e)
	{
		_AdminVM.Clear();
		Navigation.PushAsync(new AdminRegisterPage(_LangService, _AdminVM));
	}

    private void LanguageChanged(object sender, PickedEventArgs e)
    {
        Preferences.Set(Constants.Language, e.PickedItem);
        Services.LanguageService.CheckLanguage();
    }

    private void ProcessInternalMsg(string msg)
    {
        if (msg == "language-changed")
        {
            _ProductName.Text = LanguageService.StringForKey("Product");

            _AlreadyRegistered.Text = LanguageService.StringForKey("AlreadyRegistered");
            _EmployeeLogin.Text = LanguageService.StringForKey("EmployeeLogin");
            _EmployerLogin.Text = LanguageService.StringForKey("AdminLogin");
            _Register.Text = LanguageService.StringForKey("Register");
            _OR.Text = LanguageService.StringForKey("OR");

            _UserLoginVM.Username.Placeholder = LanguageService.StringForKey("Username");
            _UserLoginVM.Password.Placeholder = LanguageService.StringForKey("Password");
            _UserLoginVM.AdminID.Placeholder = LanguageService.StringForKey("CompanyId");

            _AdminLoginVM.Username.Placeholder = LanguageService.StringForKey("Username");
            _AdminLoginVM.Password.Placeholder = LanguageService.StringForKey("Password");

            _AdminVM.Username.Placeholder = LanguageService.StringForKey("Username");
            _AdminVM.Password.Placeholder = LanguageService.StringForKey("Password");
            _AdminVM.Email.Placeholder = LanguageService.StringForKey("Email");
            _AdminVM.VerificationCode.Placeholder = LanguageService.StringForKey("VerificationCode");
        }
    }
    #endregion
}