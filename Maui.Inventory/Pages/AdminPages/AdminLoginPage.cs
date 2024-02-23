using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

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
        _Login.Clicked += Login;
    }
    ~AdminLoginPage()
    {
        _Login.Clicked -= Login;
    }
    #endregion

    #region Override
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Login(object sender, EventArgs e)
    {
        if (_Login.Text == _LangService.StringForKey("Loading"))
        {
            return;
        }

        _Login.Text = _LangService.StringForKey("Loading");
        bool loggedIn = await _AdminLoginVM.Login();
        if (loggedIn)
        {
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