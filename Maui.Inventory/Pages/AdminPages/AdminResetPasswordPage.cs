using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.ViewModels.AdminVM;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminResetPasswordPage : PopupPage
{
    #region Private Properties
    private AdminResetPasswordViewModel _ViewModel => (AdminResetPasswordViewModel)BindingContext;
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
    private readonly MaterialEntry _VerificationCode;
    private readonly MaterialEntry _NewPassword;

    private readonly Label _StepOne = new() { FontSize = 21, FontAttributes = FontAttributes.Bold };
    private readonly Label _StepTwo = new() { FontSize = 21, FontAttributes = FontAttributes.Bold };

    private readonly FloatingActionButton _UpdatePassword = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended
    };
    #endregion

    #region Constructor
    public AdminResetPasswordPage(
        ILanguageService languageService,
        AdminResetPasswordViewModel adminPasswordVM) : base(languageService)
    {
        _LangService = languageService;
        BindingContext = adminPasswordVM;

        _Title.Text = _LangService.StringForKey("ResetPassword");

        _StepOne.Text = _LangService.StringForKey("StepOne");
        _StepTwo.Text = _LangService.StringForKey("StepTwo");

        _VerificationCode = new(_ViewModel.VerificationCode);
        _NewPassword = new(_ViewModel.NewPassword);

        _NewPassword.ShowStatus(
            _LangService.StringForKey("NewPasswordInstruction"),
            MaterialIcon.Info,
            Application.Current.Resources["Primary"] as Color,
            false);

        _CloseIcon.TapGesture(async () =>
        {
            await _CloseIcon.ScaleTo(0.95, 70);
            await _CloseIcon.ScaleTo(1.0, 70);

            await Navigation.PopModalAsync();
        });

        _ContentScroll.Content = _ContentLayout;

        _EntryContainer.Add(_StepOne);
        _EntryContainer.Add(_VerificationCode);

        _ContentLayout.Children.Add(_CloseIcon.Row(0).Column(2));
        _ContentLayout.Children.Add(_Title.Row(0).Column(1));
        _ContentLayout.Children.Add(_EntryContainer.Row(1).Column(0).ColumnSpan(3));
        _ContentLayout.Children.Add(_UpdatePassword.Row(2).Column(0).ColumnSpan(3));

        _UpdatePassword.Text = _LangService.StringForKey("VerifyCode");

        PopupStyle = PopupStyle.BottomSheet;
        PopupContent = _ContentScroll;
        _UpdatePassword.Clicked += UpdatePassword;
    }
    ~AdminResetPasswordPage()
    {
        _UpdatePassword.Clicked -= UpdatePassword;
    }
    #endregion

    #region Overrides
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _ViewModel.GetDetails();
        _VerificationCode.ShowStatus(
            string.Format(_LangService.StringForKey("EnterCodeSentTo"), _ViewModel.CurrentAdmin.Email),
            MaterialIcon.Info,
            Application.Current.Resources["Primary"] as Color,
            false);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void UpdatePassword(object sender, ClickedEventArgs e)
    {
        if (_UpdatePassword.Text == _LangService.StringForKey("Loading"))
        {
            return;
        }

        if (_UpdatePassword.Text == _LangService.StringForKey("VerifyCode"))
        {
            _UpdatePassword.Text = _LangService.StringForKey("Loading");
            bool verified = await _ViewModel.VerifyCode();
            if (verified)
            {
                _EntryContainer.Add(_StepTwo);
                _EntryContainer.Add(_NewPassword);
                _UpdatePassword.Text = _LangService.StringForKey("Save");
            }
            else
            {
                _UpdatePassword.Text = _LangService.StringForKey("VerifyCode");
            }
        }
        else if (_UpdatePassword.Text == _LangService.StringForKey("Save"))
        {
            if (string.IsNullOrEmpty(_ViewModel.NewPassword.Text))
            {
                _NewPassword.ShowStatus(
                    _LangService.StringForKey("Required"),
                    MaterialIcon.Info,
                    Colors.Red,
                    false);
            }
            else
            {
                _UpdatePassword.Text = _LangService.StringForKey("Loading");
                bool saved = await _ViewModel.SavePassword();
                if (saved)
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    _UpdatePassword.Text = _LangService.StringForKey("Save");
                }
            }
        }
    }
    #endregion
}
