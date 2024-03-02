using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels.UserVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.UserPages;

public class UserProfilePage : BasePage
{
    #region Private Properties
    private UserProfileViewModel _ViewModel => (UserProfileViewModel) BindingContext;
    private readonly ILanguageService _LanguageService;
    private readonly ScrollView _ContentScroll = new();
    private readonly VerticalStackLayout _ContentLayout = new()
    {
        Padding = 16,
        Spacing = 12
    };
    private readonly MaterialEntry _Username;
    private readonly MaterialEntry _CompanyId;
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
    #endregion

    #region Constructor
    public UserProfilePage(
        ILanguageService languageService,
        UserProfileViewModel userProfileVM) : base(languageService)
    {
        BindingContext = userProfileVM;
        _LanguageService = languageService;

        _Username = new(_ViewModel.Username);
        _CompanyId = new(_ViewModel.AdminID);

        _Username.IsDisabled = true;
        _CompanyId.IsDisabled = true;

        Title = _LanguageService.StringForKey("Profile");
        _Logout.Text = _LanguageService.StringForKey("Logout");
        _DarkModeSwitch.Text = _LanguageService.StringForKey("DarkMode");
        _LanguagePicker.Text = _LanguageService.StringForKey("Language");
        _Customize.Text = _LanguageService.StringForKey("Customize");
        _Options.Text = _LanguageService.StringForKey("Options");

        _ContentLayout.Add(_Username);
        _ContentLayout.Add(_CompanyId);

        _ContentLayout.Add(_Customize);
        _ContentLayout.Add(_DarkModeSwitch);
        _ContentLayout.Add(_LanguagePicker);
        _ContentLayout.Add(_Options);

        _ContentLayout.Add(_Logout);

        _ContentScroll.Content = _ContentLayout;
        Content = _ContentScroll;

        _DarkModeSwitch.Toggled += DarkModeToggled;
        _Logout.Clicked += Logout;
        _LanguagePicker.PickedItem += LangChanged;

        WeakReferenceMessenger.Default.Register<InternalMessage>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => InternalMessageReceived(msg));
        });
    }
    ~UserProfilePage()
    {
        WeakReferenceMessenger.Default.Unregister<InternalMessage>(this);
        _Logout.Clicked -= Logout;
        _DarkModeSwitch.Toggled -= DarkModeToggled;
        _LanguagePicker.PickedItem -= LangChanged;
    }
    #endregion

    #region Overrides
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _ViewModel.GetProfile();
        _DarkModeSwitch.IsToggled = _ViewModel.IsDarkModeOn;

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
        await _ViewModel.Logout();
        WeakReferenceMessenger.Default.Send(new InternalMessage(AccessMessage.UserLogout));
    }

    private async void DarkModeToggled(object sender, ToggledEventArgs e)
    {
        _ViewModel.IsDarkModeOn = e.Value;
        await _ViewModel.Edit();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UIUtils.ToggleDarkMode(_ViewModel.IsDarkModeOn);
            _DarkModeSwitch.TextColor = Application.Current.Resources["TextColor"] as Color;
            _LanguagePicker.TextColor = Application.Current.Resources["TextColor"] as Color;
        });
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
            Title = _LanguageService.StringForKey("Profile");
            _Logout.Text = _LanguageService.StringForKey("Logout");
            _DarkModeSwitch.Text = _LanguageService.StringForKey("DarkMode");
            _LanguagePicker.Text = _LanguageService.StringForKey("Language");
            _Customize.Text = _LanguageService.StringForKey("Customize");
            _Options.Text = _LanguageService.StringForKey("Options");

            _ViewModel.Username.Placeholder = _LanguageService.StringForKey("Username");
            _ViewModel.AdminID.Placeholder = _LanguageService.StringForKey("CompanyId");
        }
    }
    #endregion
}
