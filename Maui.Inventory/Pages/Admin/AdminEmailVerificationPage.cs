﻿using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Models;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory;

public class AdminEmailVerificationPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private AdminRegisterViewModel _AdminVM => (AdminRegisterViewModel) BindingContext;
    private readonly Label _Instructions = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.None,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly Label _EmailLabel = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly Label _InstructionsFinal = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.None,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly MaterialEntry _CodeEntry;
    private readonly Grid _OuterLayout = new()
    {
        Padding = 16,
        RowDefinitions = Rows.Define(Star, Auto)
    };
    private readonly ScrollView _ContentScroll = new();
    private readonly VerticalStackLayout _ContentLayout = new()
    {
        Spacing = 12
    };
    private readonly FloatingActionButton _Register = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
		TextColor = Colors.White,
		FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly Label _VerifiedEmail = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start
    };
    private readonly MaterialImage _VerifiedIcon = new()
    {
        Icon = MaterialIcon.Check_circle,
        IconColor = Colors.Green,
        IconSize = 18
    };
    private readonly Label _ReviewLabel = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start
    };
    private readonly MaterialEntry _Username;
    private readonly MaterialEntry _Password;
    private readonly MaterialEntry _Email;
    #endregion

    #region Constructor
    public AdminEmailVerificationPage(
        ILanguageService languageService,
        AdminRegisterViewModel adminVM) : base(languageService)
    {
        _LangService = languageService;
        BindingContext = adminVM;

        _CodeEntry = new(_AdminVM.VerificationCode);
        _Username = new(_AdminVM.Username);
        _Password = new(_AdminVM.Password);
        _Email = new(_AdminVM.Email);

        _Password.IsDisabled = true;
        _Email.IsDisabled = true;

        _VerifiedEmail.Text = _LangService.StringForKey("EmailVerified");
        _ReviewLabel.Text = _LangService.StringForKey("Review");

        Title = _LangService.StringForKey("Verify");
        _Register.Text = _LangService.StringForKey("VerifyEmail");

        _Instructions.Text = _LangService.StringForKey("VerificationCodeSent");
        _EmailLabel.Text = _AdminVM.Email.Text;
        _InstructionsFinal.Text = _LangService.StringForKey("VerificationCodeEnter");

        _ContentLayout.Add(_Instructions);
        _ContentLayout.Add(_EmailLabel);
        _ContentLayout.Add(_InstructionsFinal);
        _ContentLayout.Add(_CodeEntry);

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
    private async void RegisterClicked(object sender, EventArgs e)
    {
        if (_Register.Text == _LangService.StringForKey("Loading"))
        {
            return;
        }

        if (_Register.Text == _LangService.StringForKey("VerifyEmail"))
        {
            _Register.Text = _LangService.StringForKey("Loading");
            bool verified = await _AdminVM.VerifyCode();
            if (verified)
            {
                _CodeEntry.IsDisabled = true;
                _ContentLayout.Add(new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, 20),
                    ColumnSpacing = 8,
                    Children = 
                    {
                        _VerifiedEmail.Column(0),
                        _VerifiedIcon.Column(1)
                    }
                });
                _ContentLayout.Add(_ReviewLabel);
                _ContentLayout.Add(_Username);
                _ContentLayout.Add(_Password);
                _ContentLayout.Add(_Email);

                _Register.Text = _LangService.StringForKey("FinishRegistration");
            }
            else
            {
                _Register.Text = _LangService.StringForKey("VerifyEmail");
            }
        }
        else if (_Register.Text == _LangService.StringForKey("FinishRegistration"))
        {
            _Register.Text = _LangService.StringForKey("Loading");
            RegistrationResponse registered = await _AdminVM.Register();
            switch (registered)
            {
                case RegistrationResponse.SuccessfullyRegistered:
                    _Register.Text = _LangService.StringForKey("Login");
                    break;
                case RegistrationResponse.AlreadyExists:
                    _Username.ShowStatus(_LangService.StringForKey("UsernameInUse"), MaterialIcon.Info, Colors.Red);
                    _Register.Text = _LangService.StringForKey("FinishRegistration");
                    break;
                default:
                    _Register.Text = _LangService.StringForKey("FinishRegistration");
                    break;
            }
        }
        else if (_Register.Text == _LangService.StringForKey("Login"))
        {
            _Register.Text = _LangService.StringForKey("Loading");
            bool loggedIn = await _AdminVM.Login();
            if (loggedIn)
            {
                WeakReferenceMessenger.Default.Send(new InternalMessage(AccessMessage.AdminSignedIn));
            }
            else
            {
                _Register.Text = _LangService.StringForKey("Login");
            }
        }
    }
    #endregion
}
