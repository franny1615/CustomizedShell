using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminRegisterPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private AdminRegisterViewModel _AdminVM => (AdminRegisterViewModel)BindingContext;
    private readonly Grid _OuterLayout = new()
    {
        RowDefinitions = Rows.Define(Star, Auto),
        Padding = 16,
        RowSpacing = 12
    };
    private readonly ScrollView _ContentScroll = new();
    private readonly VerticalStackLayout _ContentLayout = new()
    {
        Spacing = 12,
    };
    private readonly MaterialEntry _Username;
    private readonly MaterialEntry _Password;
    private readonly MaterialEntry _Email;
    private readonly FloatingActionButton _Register = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
		TextColor = Colors.White,
		FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly Label _RegisterSample = new()
    {
        FontSize = 14,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    #endregion

    #region Constructor
    public AdminRegisterPage(
        ILanguageService languageService,
        AdminRegisterViewModel adminVM) : base(languageService)
    {
        BindingContext = adminVM;

        _Username = new(_AdminVM.Username);
        _Password = new(_AdminVM.Password);
        _Email = new(_AdminVM.Email);

        _LanguageService = languageService;

        Title = _LanguageService.StringForKey("Register");
        _Register.Text = _LanguageService.StringForKey("Submit");
        _RegisterSample.Text = LanguageService.StringForKey("AccountCreation");

        _Email.ShowStatus(
            _LanguageService.StringForKey("SampleEmail"), 
            MaterialIcon.Info, 
            Application.Current.Resources["Primary"] as Color,
            updateBorder: false);

        _ContentLayout.Add(_RegisterSample);
        _ContentLayout.Add(_Username);
        _ContentLayout.Add(_Password);
        _ContentLayout.Add(_Email);

        _ContentScroll.Content = _ContentLayout;

        _OuterLayout.Children.Add(_ContentScroll.Row(0));
        _OuterLayout.Children.Add(_Register.Row(1));

        Content = _OuterLayout;
        _Register.Clicked += RegisterClicked;

        WeakReferenceMessenger.Default.Register<InternalMessage>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => ProcessInternalMsg(msg.Value.ToString()));
        });
    }
    ~AdminRegisterPage()
    {
        WeakReferenceMessenger.Default.Unregister<InternalMessage>(this);
        _Register.Clicked -= RegisterClicked;
    }
    #endregion

    #region Overrides
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
    private async void RegisterClicked(object sender, EventArgs e)
    {
        if (_Register.Text == _LanguageService.StringForKey("Loading"))
        {
            return;
        }

        bool haveUsername = !string.IsNullOrEmpty(_AdminVM.Username.Text);
        bool havePassword = !string.IsNullOrEmpty(_AdminVM.Password.Text);
        bool haveEmail = !string.IsNullOrEmpty(_AdminVM.Email.Text);
        bool emailProperFormat = StringUtils.IsValidEmail(_AdminVM.Email.Text);

        if (haveUsername)
            _Username.ShowStatus(null, null, Colors.Green);
        else
            _Username.ShowStatus(_LanguageService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
        
        if (havePassword)
            _Password.ShowStatus(null, null, Colors.Green);
        else
            _Password.ShowStatus(_LanguageService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);

        if (haveEmail && emailProperFormat)
            _Email.ShowStatus(null, null, Colors.Green);
        else
        {
            if (!emailProperFormat && haveEmail)
            {
                _Email.ShowStatus(_LanguageService.StringForKey("InvalidEmail"), MaterialIcon.Info, Colors.Red);
            }
            else
            {
                _Email.ShowStatus(_LanguageService.StringForKey("Required"), MaterialIcon.Info, Colors.Red);
            }
        }

        if (!haveUsername || !havePassword || !haveEmail || !emailProperFormat)
        {
            return;
        }

        _Register.Text = _LanguageService.StringForKey("Loading");
        bool sentConfirmation = await _AdminVM.BeginEmailVerification();
        if (sentConfirmation)
        {
            _AdminVM.VerificationCode.Text = "";
            await Navigation.PushAsync(new AdminEmailVerificationPage(_LanguageService, _AdminVM));
        }
        _Register.Text = _LanguageService.StringForKey("Submit");
    }

    private void ProcessInternalMsg(string msg)
    {
        if (msg == "language-changed")
        {
            Title = _LanguageService.StringForKey("Register");
            _Register.Text = _LanguageService.StringForKey("Submit");
            _RegisterSample.Text = LanguageService.StringForKey("AccountCreation");

            _Email.ShowStatus(
                _LanguageService.StringForKey("SampleEmail"),
                MaterialIcon.Info,
                Application.Current.Resources["Primary"] as Color,
                updateBorder: false);

            _AdminVM.Username.Placeholder = _LanguageService.StringForKey("Username");
            _AdminVM.Password.Placeholder = _LanguageService.StringForKey("Password");
            _AdminVM.Email.Placeholder = _LanguageService.StringForKey("Email");
            _AdminVM.VerificationCode.Placeholder = _LanguageService.StringForKey("VerificationCode");
        }
    }
    #endregion
}