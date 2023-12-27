using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.Admin;

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
    private readonly MaterialEntry _Username = new()
    {
        Keyboard = Keyboard.Text,
        IsSpellCheckEnabled = false
    };
    private readonly MaterialEntry _Password = new()
    {
        IsPassword = true
    };
    private readonly MaterialEntry _Email = new()
    {
        Keyboard = Keyboard.Email,
        IsSpellCheckEnabled = false
    };
    private readonly FloatingActionButton _Register = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
		TextColor = Colors.White,
		FABStyle = FloatingActionButtonStyle.Extended
    };
    #endregion

    #region Constructor
    public AdminRegisterPage(
        ILanguageService languageService,
        AdminRegisterViewModel adminVM) : base(languageService)
    {
        BindingContext = adminVM;

        _LanguageService = languageService;

        Title = _LanguageService.StringForKey("Register");
        _Register.Text = _LanguageService.StringForKey("Submit");

        _Username.Placeholder = LanguageService.StringForKey("Username");
        _Password.Placeholder = LanguageService.StringForKey("Password");
        _Email.Placeholder = LanguageService.StringForKey("Email");
        _Email.SupportingText = LanguageService.StringForKey("SampleEmail");

        _ContentLayout.Add(_Username);
        _ContentLayout.Add(_Password);
        _ContentLayout.Add(_Email);

        _ContentScroll.Content = _ContentLayout;

        _OuterLayout.Children.Add(_ContentScroll.Row(0));
        _OuterLayout.Children.Add(_Register.Row(1));

        Content = _OuterLayout;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Register.Clicked += RegisterClicked;
    }

    protected override void OnDisappearing()
    {
        _Register.Clicked -= RegisterClicked;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private void RegisterClicked(object sender, EventArgs e)
    {
        bool haveUsername = !string.IsNullOrEmpty(_Username.Text);
        bool havePassword = !string.IsNullOrEmpty(_Password.Text);
        bool haveEmail = !string.IsNullOrEmpty(_Email.Text);
        bool emailProperFormat = StringUtils.IsValidEmail(_Email.Text);

        _Username.BorderColor = haveUsername ? Colors.Green : Colors.Red;
        _Password.BorderColor = havePassword ? Colors.Green : Colors.Red;
        _Email.BorderColor = (haveEmail && emailProperFormat) ? Colors.Green : Colors.Red;

        _Username.SupportingText = haveUsername ? "" : LanguageService.StringForKey("Required");
        _Password.SupportingText = havePassword ? "" : LanguageService.StringForKey("Required");
        _Email.SupportingText = haveEmail ? "" : LanguageService.StringForKey("Required");

        _Email.SupportingText = emailProperFormat ? "" : LanguageService.StringForKey("InvalidEmail");

        if (!haveUsername || !havePassword || !haveEmail)
        {
            return;
        }
    }
    #endregion
}