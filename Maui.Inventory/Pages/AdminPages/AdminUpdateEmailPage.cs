using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminUpdateEmailPage : PopupPage
{
    #region Private Properties
    private AdminUpdateEmailViewModel _AdminUpdateEmailVM => (AdminUpdateEmailViewModel) BindingContext;
    private readonly ILanguageService _LangService;
    private readonly ScrollView _ContentScroll = new();
    private readonly MaterialImage _CloseIcon = new()
    {
        Icon = MaterialIcon.Close,
        IconSize = 25,
        IconColor = Application.Current.Resources["TextColor"] as Color,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly Label _Title = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(30, Star, Auto),
        ColumnDefinitions = Columns.Define(30, Star, 30),
        Margin = new Thickness(0, 0, 0, 32),
        ColumnSpacing = 8,
        RowSpacing = 12,
    };
    private readonly VerticalStackLayout _EntryContainer = new()
    {
        Spacing = 12
    };
    private readonly Label _StepOne = new() { FontSize = 21, FontAttributes = FontAttributes.Bold };
    private readonly Label _StepTwo = new() { FontSize = 21, FontAttributes = FontAttributes.Bold };
    private readonly MaterialEntry _Email;
    private readonly MaterialEntry _VerificationCode;
    private readonly FloatingActionButton _UpdateEmail = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended
    };
    #endregion

    #region Constructor
    public AdminUpdateEmailPage(
        ILanguageService languageService,
        AdminUpdateEmailViewModel emailUpdateVM) : base(languageService)
    {
        _LangService = languageService;
        BindingContext = emailUpdateVM;

        _ContentScroll.Content = _ContentLayout;

        _Email = new(_AdminUpdateEmailVM.Email);
        _VerificationCode = new(_AdminUpdateEmailVM.VerificationCode);

        _Title.Text = _LangService.StringForKey("UpdateEmail");
        
        _Email.ShowStatus(
            _LangService.StringForKey("EnterNewEmail"),
            MaterialIcon.Info,
            Application.Current.Resources["Primary"] as Color,
            false);
        _VerificationCode.ShowStatus(
            _LangService.StringForKey("EnterCode"),
            MaterialIcon.Info,
            Application.Current.Resources["Primary"] as Color,
            false);

        _StepOne.Text = _LangService.StringForKey("StepOne");
        _StepTwo.Text = _LangService.StringForKey("StepTwo");

        _UpdateEmail.Text = _LangService.StringForKey("SendCode");

        _EntryContainer.Add(_StepOne);
        _EntryContainer.Add(_Email);

        _CloseIcon.TapGesture(async () => 
        {
            await _CloseIcon.ScaleTo(0.95, 70);
            await _CloseIcon.ScaleTo(1.0, 70);

            await Navigation.PopModalAsync();
        });

        _ContentLayout.Children.Add(_CloseIcon.Row(0).Column(2));
        _ContentLayout.Children.Add(_Title.Row(0).Column(1));
        _ContentLayout.Children.Add(_EntryContainer.Row(1).Column(0).ColumnSpan(3));
        _ContentLayout.Children.Add(_UpdateEmail.Row(2).Column(0).ColumnSpan(3));

        PopupStyle = PopupStyle.BottomSheet;
        PopupContent = _ContentScroll;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _UpdateEmail.Clicked += UpdateEmail;
    }

    protected override void OnDisappearing()
    {
        _UpdateEmail.Clicked -= UpdateEmail;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void UpdateEmail(object sender, ClickedEventArgs e)
    {
        if (string.IsNullOrEmpty(_AdminUpdateEmailVM.Email.Text))
        {
            _Email.ShowStatus(
                _LangService.StringForKey("Required"),
                MaterialIcon.Info,
                Colors.Red);

            return;
        }

        if (!StringUtils.IsValidEmail(_AdminUpdateEmailVM.Email.Text))
        {
            _Email.ShowStatus(
                _LangService.StringForKey("InvalidEmail"),
                MaterialIcon.Info,
                Colors.Red);

            return;
        }

        if (_UpdateEmail.Text == _LangService.StringForKey("Loading"))
        {
            return;
        }

        if (_UpdateEmail.Text == _LangService.StringForKey("SendCode"))
        {
            _UpdateEmail.Text = _LangService.StringForKey("Loading");
            bool sentCode = await _AdminUpdateEmailVM.SendCode();
            if (sentCode)
            {
                _EntryContainer.Add(_StepTwo);
                _EntryContainer.Add(_VerificationCode);
                _UpdateEmail.Text = _LangService.StringForKey("VerifyCode");
            }
            else
            {
                _UpdateEmail.Text = _LangService.StringForKey("SendCode");
            }
        }
        else if (_UpdateEmail.Text == _LangService.StringForKey("VerifyCode"))
        {
            _UpdateEmail.Text = _LangService.StringForKey("Loading");
            bool verified = await _AdminUpdateEmailVM.VerifyCode();
            if (verified)
            {
                _UpdateEmail.Text = _LangService.StringForKey("Save");
            }
            else
            {
                _UpdateEmail.Text = _LangService.StringForKey("VerifyCode");
            }
        }
        else if (_UpdateEmail.Text == _LangService.StringForKey("Save"))
        {
            _UpdateEmail.Text = _LangService.StringForKey("Loading");
            bool updated = await _AdminUpdateEmailVM.SaveEmail();
            if (updated)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                _UpdateEmail.Text = _LangService.StringForKey("Save");
            }
        }
    }
    #endregion
}
