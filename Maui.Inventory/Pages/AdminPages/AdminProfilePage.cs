using System.Net.Http.Headers;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminProfilePage : BasePage
{
    #region Private Properties
    private AdminProfileViewModel _AdminProfileVM => (AdminProfileViewModel) BindingContext;
    private readonly AdminUpdateEmailViewModel _AdminUpdateEmail;
    private readonly AdminResetPasswordViewModel _AdminResetVM;
    private readonly ILanguageService _LangService;
    private readonly ScrollView _ContentScroll = new();
    private readonly VerticalStackLayout _ContentLayout = new()
    {
        Padding = 16,
        Spacing = 12
    };
    private readonly MaterialEntry _Username;
    private readonly MaterialEntry _Email;
    private readonly MaterialEntry _CompanyId;
    private readonly MaterialEntry _LicenseId;
    private readonly FloatingActionButton _UpdateEmail = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
		TextColor = Colors.White,
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Email, Colors.White),
		FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly FloatingActionButton _ResetPassword = new()
    {
        FABBackgroundColor = Application.Current.Resources["PrimaryShade"] as Color,
		TextColor = Colors.White,
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Password, Colors.White),
		FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly FloatingActionButton _Logout = new()
    {
        FABBackgroundColor = Colors.Red,
		TextColor = Colors.White,
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Logout, Colors.White),
		FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly Grid _DarkModeToggleLayout = new()
    {
        ColumnDefinitions = Columns.Define(30, Star, Auto),
        ColumnSpacing = 8
    };
    private readonly MaterialImage _DarkModeIcon = new()
    {
        Icon = MaterialIcon.Dark_mode,
        IconSize = 25,
        IconColor = Colors.DarkGray
    };
    private readonly Label _DarkModeLabel = new()
    {
        FontSize = 18,
        VerticalOptions = LayoutOptions.Center,
        FontAttributes = FontAttributes.Bold,
    };
    private readonly Switch _DarkModeSwitch = new()
    {
        ThumbColor = Application.Current.Resources["Primary"] as Color
    };
    private readonly Label _LicenseExpiredLabel = new()
    {
        FontSize = 16,
        TextColor = Colors.Red,
        HorizontalOptions = LayoutOptions.Center,
    };
    #endregion

    #region Constructor
    public AdminProfilePage(
        ILanguageService languageService,
        AdminProfileViewModel adminProfileVM,
        AdminUpdateEmailViewModel adminUpdateEmailVM,
        AdminResetPasswordViewModel adminResetPassVM) : base(languageService)
    {
        _LangService = languageService;
        BindingContext = adminProfileVM;
        _AdminUpdateEmail = adminUpdateEmailVM;
        _AdminResetVM = adminResetPassVM;

        _Username = new(_AdminProfileVM.Username);
        _Email = new(_AdminProfileVM.Email);
        _CompanyId = new(_AdminProfileVM.CompanyId);
        _LicenseId = new(_AdminProfileVM.LicenseId);

        _CompanyId.ShowStatus(
            _LangService.StringForKey("CompanyIdSupportText"),
            MaterialIcon.Info,
            Application.Current.Resources["Primary"] as Color,
            updateBorder: false);

        _Email.ShowStatus(
            _LangService.StringForKey("EmailSupportText"),
            MaterialIcon.Info,
            Application.Current.Resources["Primary"] as Color,
            updateBorder: false);

        _Username.IsDisabled = true;
        _Email.IsDisabled = true;
        _CompanyId.IsDisabled = true;
        _LicenseId.IsDisabled = true;

        Title = _LangService.StringForKey("Profile");
        _UpdateEmail.Text = _LangService.StringForKey("UpdateEmail");
        _ResetPassword.Text = _LangService.StringForKey("ResetPassword");
        _Logout.Text = _LangService.StringForKey("Logout"); 
        _LicenseExpiredLabel.Text = _LangService.StringForKey("License Expired");

        _DarkModeToggleLayout.Children.Add(_DarkModeIcon.Column(0));
        _DarkModeToggleLayout.Children.Add(_DarkModeLabel.Column(1));
        _DarkModeToggleLayout.Children.Add(_DarkModeSwitch.Column(2));

        _DarkModeLabel.Text = _LangService.StringForKey("DarkMode");

        _ContentLayout.Add(_Username);
        _ContentLayout.Add(_Email);
        _ContentLayout.Add(_CompanyId);
        _ContentLayout.Add(_LicenseId);

        if (!AccessControl.IsLicenseValid)
        {
            _ContentLayout.Add(_LicenseExpiredLabel);
        }

        _ContentLayout.Add(UIUtils.HorizontalRuleWithText(_LangService.StringForKey("Customize")));

        _ContentLayout.Add(_DarkModeToggleLayout);

        _ContentLayout.Add(UIUtils.HorizontalRuleWithText(_LangService.StringForKey("Options")));

        _ContentLayout.Add(_UpdateEmail);
        _ContentLayout.Add(_ResetPassword);
        _ContentLayout.Add(_Logout);

        _ContentScroll.Content = _ContentLayout;
        Content = _ContentScroll;

        _DarkModeSwitch.Toggled += DarkModeToggled;
        _Logout.Clicked += Logout;
        _UpdateEmail.Clicked += UpdateEmail;
        _ResetPassword.Clicked += ResetPassword;
    }
    ~AdminProfilePage()
    {
        _Logout.Clicked -= Logout;
        _DarkModeSwitch.Toggled -= DarkModeToggled;
        _UpdateEmail.Clicked -= UpdateEmail;
        _ResetPassword.Clicked -= ResetPassword;
    }
    #endregion

    #region Overrides
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _AdminProfileVM.GetProfile();
        _DarkModeSwitch.IsToggled = _AdminProfileVM.IsDarkModeOn;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Logout(object sender, EventArgs e)
    {
        await _AdminProfileVM.Logout();
        WeakReferenceMessenger.Default.Send(new InternalMessage(AccessMessage.AdminLogout));
    }

    private async void DarkModeToggled(object sender, ToggledEventArgs e)
    {
        _AdminProfileVM.IsDarkModeOn = e.Value;
        await _AdminProfileVM.SaveDarkModeSettings();
        MainThread.BeginInvokeOnMainThread(() => 
        {
            UIUtils.ToggleDarkMode(_AdminProfileVM.IsDarkModeOn);
        });
    }

    private async void UpdateEmail(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AdminUpdateEmailPage(_LangService, _AdminUpdateEmail));
    }

    private async void ResetPassword(object sender, EventArgs e)
    {
        _ResetPassword.Text = _LangService.StringForKey("Loading");
        if (await _AdminProfileVM.SendCode())
        {
            await Navigation.PushModalAsync(new AdminResetPasswordPage(_LangService, _AdminResetVM));
        }
        _ResetPassword.Text = _LangService.StringForKey("ResetPassword");
    }
    #endregion
}