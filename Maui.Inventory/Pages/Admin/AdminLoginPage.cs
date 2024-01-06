using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.Admin;

public class AdminLoginPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private readonly AdminLoginViewModel _AdminLoginVM;
    private readonly MaterialEntry _Username;
    private readonly MaterialEntry _Password;
    private readonly FloatingActionButton _Login = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
		TextColor = Colors.White,
		FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly ScrollView _ContentScroll = new();
    private readonly VerticalStackLayout _ContentContainer = new()
    {
        Spacing = 12
    };
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Star, Auto),
        RowSpacing = 8,
        Padding = 16
    };
    #endregion

    #region Constructor
    public AdminLoginPage(
        ILanguageService languageService,
        AdminLoginViewModel adminLoginVM) : base(languageService)
    {
        _LangService = languageService;
        _AdminLoginVM = adminLoginVM;

        Title = _LangService.StringForKey("OwnerLogin");

        _Username = new(_AdminLoginVM.Username);
        _Password = new(_AdminLoginVM.Password);

        _Login.Text = _LangService.StringForKey("Login");

        _ContentContainer.Add(_Username);
        _ContentContainer.Add(_Password);

        _ContentScroll.Content = _ContentContainer;

        _ContentLayout.Children.Add(_ContentScroll.Row(0));
        _ContentLayout.Children.Add(_Login.Row(1));

        Content = _ContentLayout;
    }
    #endregion

    #region Override
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Login.Clicked += Login;
    }

    protected override void OnDisappearing()
    {
        _Login.Clicked -= Login;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Login(object sender, EventArgs e)
    {
        _Login.Text = _LangService.StringForKey("Loading");
        bool loggedIn = await _AdminLoginVM.Login();
        if (loggedIn)
        {
            // TODO: need a way to check if their license has expired and they need to re-purchase subscription
            // change backend to not check if license is valid on login, instead 
            // pass back a new LicenseValid boolean as part of Admin model
            // based on that flag, disable app features. 
            // user should have free access to all data. 
            // if they do not pay, omit adding new data to servers, and omit barcode generation and printing features.
            WeakReferenceMessenger.Default.Send(new InternalMessage(AccessMessage.AdminSignedIn));
        }
        else
        {
            _Username.ShowStatus(_LangService.StringForKey("UsernamePossiblyWrong"), MaterialIcon.Info, Colors.Red);
            _Password.ShowStatus(_LangService.StringForKey("PasswordPossiblyWrong"), MaterialIcon.Info, Colors.Red);
        }
        _Login.Text = _LangService.StringForKey("Login");
    }
    #endregion
}