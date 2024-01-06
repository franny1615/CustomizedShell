using System.Net.Http.Headers;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;

namespace Maui.Inventory.Pages.Admin;

public class AdminProfilePage : BasePage
{
    #region Private Properties
    private AdminProfileViewModel _AdminProfileVM => (AdminProfileViewModel) BindingContext;
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
    #endregion

    #region Constructor
    public AdminProfilePage(
        ILanguageService languageService,
        AdminProfileViewModel adminProfileVM) : base(languageService)
    {
        _LangService = languageService;
        BindingContext = adminProfileVM;

        _Username = new(_AdminProfileVM.Username);
        _Email = new(_AdminProfileVM.Email);
        _CompanyId = new(_AdminProfileVM.CompanyId);
        _LicenseId = new(_AdminProfileVM.LicenseId);

        _CompanyId.ShowStatus(
            _LangService.StringForKey("CompanyIdSupportText"),
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

        _ContentLayout.Add(_Username);
        _ContentLayout.Add(_Email);
        _ContentLayout.Add(_CompanyId);
        _ContentLayout.Add(_LicenseId);

        _ContentLayout.Add(UIUtils.HorizontalRuleWithText(_LangService.StringForKey("Options")));

        _ContentLayout.Add(_UpdateEmail);
        _ContentLayout.Add(_ResetPassword);
        _ContentLayout.Add(_Logout);

        _ContentScroll.Content = _ContentLayout;
        Content = _ContentScroll;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _AdminProfileVM.GetProfile();

        _Logout.Clicked += Logout;
    }

    protected override void OnDisappearing()
    {
        _Logout.Clicked -= Logout;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Logout(object sender, EventArgs e)
    {
        await _AdminProfileVM.Logout();
        WeakReferenceMessenger.Default.Send(new InternalMessage(AccessMessage.AdminLogout));
    }
    #endregion
}