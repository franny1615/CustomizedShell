using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Inventory.ViewModels.AdminVM;

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
    private readonly MaterialEntry _LicenseExpirationDate; 
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
    private readonly FloatingActionButton _DeleteAccount = new()
    {
        FABBackgroundColor = Application.Current.Resources["PrimaryShade"] as Color,
        TextColor = Colors.White,
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Delete, Colors.White),
        FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly FloatingActionButton _Logout = new()
    {
        FABBackgroundColor = Colors.Red,
		TextColor = Colors.White,
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Logout, Colors.White),
		FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly MaterialToggle _DarkModeSwitch = new()
    {
        Icon = MaterialIcon.Dark_mode,
        IconColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Application.Current.Resources["TextColor"] as Color
    };
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
    private readonly HorizontalRule _Customize = new();
    private readonly HorizontalRule _Options = new();
    private readonly ToolbarItem _Feedback = new();
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
        _LicenseExpirationDate = new(_AdminProfileVM.LicenseExpirationDate);

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
        _LicenseExpirationDate.IsDisabled = true;

        Title = _LangService.StringForKey("Profile");
        _UpdateEmail.Text = _LangService.StringForKey("UpdateEmail");
        _ResetPassword.Text = _LangService.StringForKey("ResetPassword");
        _Logout.Text = _LangService.StringForKey("Logout"); 
        _DeleteAccount.Text = _LangService.StringForKey("Delete Account");
        _Customize.Text = _LangService.StringForKey("Customize");
        _Options.Text = _LangService.StringForKey("Options");

        _DarkModeSwitch.Text = _LangService.StringForKey("DarkMode");
        _LanguagePicker.Text = _LangService.StringForKey("Language");

        _ContentLayout.Add(_Username);
        _ContentLayout.Add(_Email);
        _ContentLayout.Add(_CompanyId);
        _ContentLayout.Add(_LicenseId);
        _ContentLayout.Add(_LicenseExpirationDate);

        if (!AccessControl.IsLicenseValid)
        {
            _LicenseExpirationDate.ShowStatus(_LangService.StringForKey("License Expired"), MaterialIcon.Info, Colors.Red);
        }

        _ContentLayout.Add(_Customize);

        _ContentLayout.Add(_DarkModeSwitch);
        _ContentLayout.Add(_LanguagePicker);

        _ContentLayout.Add(_Options);

        _ContentLayout.Add(_UpdateEmail);
        _ContentLayout.Add(_ResetPassword);
        _ContentLayout.Add(_DeleteAccount);
        _ContentLayout.Add(_Logout);

        _ContentScroll.Content = _ContentLayout;
        Content = _ContentScroll;

        _DarkModeSwitch.Toggled += DarkModeToggled;
        _Logout.Clicked += Logout;
        _UpdateEmail.Clicked += UpdateEmail;
        _ResetPassword.Clicked += ResetPassword;
        _DeleteAccount.Clicked += DeleteAccountClicked;
        _LanguagePicker.PickedItem += LangChanged;

        _Feedback.Text = _LangService.StringForKey("Feedback");
        _Feedback.Command = new Command(() => { Shell.Current.GoToAsync(nameof(SendFeedbackPage)); });
        ToolbarItems.Add(_Feedback);

        WeakReferenceMessenger.Default.Register<InternalMessage>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => InternalMessageReceived(msg));
        });
    }
    ~AdminProfilePage()
    {
        WeakReferenceMessenger.Default.Unregister<InternalMessage>(this);
        _Logout.Clicked -= Logout;
        _DarkModeSwitch.Toggled -= DarkModeToggled;
        _UpdateEmail.Clicked -= UpdateEmail;
        _ResetPassword.Clicked -= ResetPassword;
        _DeleteAccount.Clicked -= DeleteAccountClicked;
        _LanguagePicker.PickedItem -= LangChanged;
    }
    #endregion

    #region Overrides
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _AdminProfileVM.GetProfile();
        _DarkModeSwitch.IsToggled = _AdminProfileVM.IsDarkModeOn;

        string lang = Preferences.Get(Constants.Language, "English");
        _LanguagePicker.SelectedItem = lang;
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
            _DarkModeSwitch.TextColor = Application.Current.Resources["TextColor"] as Color;
            _LanguagePicker.TextColor = Application.Current.Resources["TextColor"] as Color;
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

    private async void DeleteAccountClicked(object sender, ClickedEventArgs e)
    {
        bool wantsToDelete = await DisplayAlert(
            _LangService.StringForKey("Delete Account"),
            _LangService.StringForKey("DeleteAccountPrompt"),
            _LangService.StringForKey("Yes"),
            _LangService.StringForKey("No"));

        if (wantsToDelete)
        {
            await _AdminProfileVM.DeleteAccount();
            WeakReferenceMessenger.Default.Send(new InternalMessage(AccessMessage.LandingPage));
        }
    }

    private void LangChanged(object sender, PickedEventArgs e)
    {
        Preferences.Set(Constants.Language, e.PickedItem);
        Services.LanguageService.CheckLanguage();
    }

    private void InternalMessageReceived(InternalMessage msg)
    {
        if (msg.Value is string message && message == "language-changed")
        {
            _AdminProfileVM.Username.Placeholder = _LangService.StringForKey("Username");
            _AdminProfileVM.Email.Placeholder = _LangService.StringForKey("Email");
            _AdminProfileVM.CompanyId.Placeholder = _LangService.StringForKey("CompanyId");
            _AdminProfileVM.LicenseId.Placeholder = _LangService.StringForKey("LicenseId");
            _AdminProfileVM.LicenseExpirationDate.Placeholder = _LangService.StringForKey("License Expiration Date");
            _Customize.Text = _LangService.StringForKey("Customize");
            _Options.Text = _LangService.StringForKey("Options");

            Title = _LangService.StringForKey("Profile");
            _UpdateEmail.Text = _LangService.StringForKey("UpdateEmail");
            _ResetPassword.Text = _LangService.StringForKey("ResetPassword");
            _Logout.Text = _LangService.StringForKey("Logout");
            _DeleteAccount.Text = _LangService.StringForKey("Delete Account");

            _DarkModeSwitch.Text = _LangService.StringForKey("DarkMode");
            _LanguagePicker.Text = _LangService.StringForKey("Language");

            _AdminUpdateEmail.VerificationCode.Placeholder = _LangService.StringForKey("VerificationCode");
            _AdminUpdateEmail.Email.Placeholder = _LangService.StringForKey("Email");

            _AdminResetVM.VerificationCode.Placeholder = _LangService.StringForKey("VerificationCode");
            _AdminResetVM.NewPassword.Placeholder = _LangService.StringForKey("NewPassword");

            _Feedback.Text = _LangService.StringForKey("Feedback");
        }
    }
    #endregion
}