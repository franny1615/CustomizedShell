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
        _DarkModeLabel.Text = _LanguageService.StringForKey("DarkMode");

        _DarkModeToggleLayout.Children.Add(_DarkModeIcon.Column(0));
        _DarkModeToggleLayout.Children.Add(_DarkModeLabel.Column(1));
        _DarkModeToggleLayout.Children.Add(_DarkModeSwitch.Column(2));

        _ContentLayout.Add(_Username);
        _ContentLayout.Add(_CompanyId);

        _ContentLayout.Add(UIUtils.HorizontalRuleWithText(_LanguageService.StringForKey("Customize")));
        _ContentLayout.Add(_DarkModeToggleLayout);
        _ContentLayout.Add(UIUtils.HorizontalRuleWithText(_LanguageService.StringForKey("Options")));

        _ContentLayout.Add(_Logout);

        _ContentScroll.Content = _ContentLayout;
        Content = _ContentScroll;
    }
    #endregion

    #region Overrides
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _DarkModeSwitch.Toggled += DarkModeToggled;
        _Logout.Clicked += Logout;

        await _ViewModel.GetProfile();
        _DarkModeSwitch.IsToggled = _ViewModel.IsDarkModeOn;
    }

    protected override void OnDisappearing()
    {
        _Logout.Clicked -= Logout;
        _DarkModeSwitch.Toggled -= DarkModeToggled;
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
        });
    }
    #endregion
}
