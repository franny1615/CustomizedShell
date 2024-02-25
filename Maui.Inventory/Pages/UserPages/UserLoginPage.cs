using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels.UserVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.UserPages;

public class UserLoginPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private UserLoginViewModel _ViewModel => (UserLoginViewModel) BindingContext;
    private readonly MaterialEntry _Username;
    private readonly MaterialEntry _Password;
    private readonly MaterialEntry _AdminID;
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
    public UserLoginPage(
        ILanguageService languageService,
        UserLoginViewModel userLoginViewModel) : base(languageService)
    {
        _LanguageService = languageService;
        BindingContext = userLoginViewModel;

        Title = _LanguageService.StringForKey("UserLogin");

        _Username = new(_ViewModel.Username);
        _Password = new(_ViewModel.Password);
        _AdminID = new(_ViewModel.AdminID);

        _Login.Text = _LanguageService.StringForKey("Login");

        _ContentContainer.Add(_AdminID);
        _ContentContainer.Add(_Username);
        _ContentContainer.Add(_Password);

        string storedAdminId = Preferences.Default.Get(Constants.AdminId, "");
        if (!string.IsNullOrEmpty(storedAdminId))
        {
            _ViewModel.AdminID.Text = storedAdminId;
        }

        _ContentScroll.Content = _ContentContainer;

        _ContentLayout.Children.Add(_ContentScroll.Row(0));
        _ContentLayout.Children.Add(_Login.Row(1));

        Content = _ContentLayout;
        _Login.Clicked += Login;
    }
    ~UserLoginPage()
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
        if (_Login.Text == _LanguageService.StringForKey("Loading"))
        {
            return;
        }

        if (string.IsNullOrEmpty(_ViewModel.AdminID.Text))
        {
            _AdminID.ShowStatus(_LanguageService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
            return;
        }    

        _Login.Text = _LanguageService.StringForKey("Loading");
        bool loggedIn = await _ViewModel.Login();
        if (loggedIn)
        {
            Preferences.Default.Set(Constants.AdminId, _ViewModel.AdminID.Text);
            WeakReferenceMessenger.Default.Send(new InternalMessage(AccessMessage.UserSignedIn));
        }
        else
        {
            _Username.ShowStatus(_LanguageService.StringForKey("UsernamePossiblyWrong"), MaterialIcon.Info, Colors.Red);
            _Password.ShowStatus(_LanguageService.StringForKey("PasswordPossiblyWrong"), MaterialIcon.Info, Colors.Red);
        }
        _Login.Text = _LanguageService.StringForKey("Login");
    }
    #endregion
}
