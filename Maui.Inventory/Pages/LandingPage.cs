using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
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

		_ActionsContainer.Add(_AlreadyRegistered);
		_ActionsContainer.Add(_EmployeeLogin);
		_ActionsContainer.Add(_EmployerLogin);
		_ActionsContainer.Add(UIUtils.HorizontalRuleWithText(
			LanguageService.StringForKey("OR")
		).Margin(new Thickness(8, 12, 8, 12)));
		_ActionsContainer.Add(_Register);

		_ContentLayout.Add(_ProductContainer.Row(0));
		_ContentLayout.Add(_ActionsContainer.Row(1).Bottom());

		Content = _ContentLayout;
	}
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
		_EmployeeLogin.Clicked += EmployeeLogin;
		_EmployerLogin.Clicked += EmployerLogin;
		_Register.Clicked += EmployerRegister;
    }

    protected override void OnDisappearing()
    {
		_EmployeeLogin.Clicked -= EmployeeLogin;
		_EmployerLogin.Clicked -= EmployerLogin;
		_Register.Clicked -= EmployerRegister;
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
	#endregion
}