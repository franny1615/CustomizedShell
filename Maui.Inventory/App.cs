using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Components;
using Maui.Inventory.Pages;
using Maui.Inventory.Pages.AdminPages;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Maui.Inventory.ViewModels.AdminVM;
using Maui.Inventory.ViewModels.UserVM;
using Maui.Inventory.Pages.UserPages;
using Maui.Components.Utilities;

namespace Maui.Inventory;

public class App : Application
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly AdminRegisterViewModel _AdminVM;
    private readonly AdminLoginViewModel _AdminLoginVM;
    private readonly UserLoginViewModel _UserLoginVM;
    private readonly AppViewModel _AppVM;
    #endregion

    #region Constructor
    public App(
        ILanguageService languageService,
        SplashViewModel splashViewModel,
        AdminRegisterViewModel adminVM,
        AdminLoginViewModel adminLoginVM,
        UserLoginViewModel userLoginVM,
        AppViewModel appVM)
    {
        _LanguageService = languageService;
        _AdminVM = adminVM;
        _AdminLoginVM = adminLoginVM;
        _UserLoginVM = userLoginVM;
        _AppVM = appVM;

        Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

        MainPage = new SplashPage(_LanguageService, splashViewModel);

        RegisterListeners();
    }
    #endregion

    #region Overrides
    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Resumed += AppResumed;
        window.Stopped += AppStopped;
        window.Created += AppCreated;

        return window;
    }

    protected override void OnStart()
    {
        base.OnStart();
        string keyString = $"{Constants.iOSAppCenterSecret}{Constants.AndroidAppCenterSecret}";
        AppCenter.Start(keyString, typeof(Analytics), typeof(Crashes));
    }

    private async void AppResumed(object sender, EventArgs e)
    {
        if ((MainPage is AdminShell || MainPage is UserShell) &&
            !await _AppVM.IsAccessTokenValid()) // were signed in at some page, now we're not
        {
            UIUtils.ToggleDarkMode(false);
            MainPage = new NavigationPage(new LandingPage(_LanguageService, _AdminVM, _AdminLoginVM, _UserLoginVM));
        }
    }

    private void AppCreated(object sender, EventArgs e) { }
    private void AppStopped(object sender, EventArgs e) { }
    #endregion

    #region Helpers
    private void RegisterListeners()
    {
        WeakReferenceMessenger.Default.Register<InternalMessage>(this, (_, message) =>
        {
            MainThread.BeginInvokeOnMainThread(() => { HandleInternalMessage(message); });
        });
    }

    private void HandleInternalMessage(InternalMessage message)
    {
        if (message.Value is AccessMessage access)
        {
            AccessControl(access);
        }
    }

    private async void AccessControl(AccessMessage access)
    {
        switch (access)
        {
            case AccessMessage.AdminSignedIn:
                UIUtils.ToggleDarkMode(await _AppVM.ShouldEnableDarkMode());
                MainPage = new AdminShell(_LanguageService);
                break;
            case AccessMessage.UserSignedIn:
                UIUtils.ToggleDarkMode(await _AppVM.ShouldEnableDarkMode());
                MainPage = new UserShell(_LanguageService);
                break;
            case AccessMessage.AdminLogout:
                _AdminLoginVM.Clear();
                UIUtils.ToggleDarkMode(false);
                
                var nav = new NavigationPage(new LandingPage(_LanguageService, _AdminVM, _AdminLoginVM, _UserLoginVM));
                await nav.PushAsync(new AdminLoginPage(_LanguageService, _AdminLoginVM));

                MainPage = nav;
                break;
            case AccessMessage.UserLogout:
                _UserLoginVM.Clear();
                UIUtils.ToggleDarkMode(false);

                var nav2 = new NavigationPage(new LandingPage(_LanguageService, _AdminVM, _AdminLoginVM, _UserLoginVM));
                await nav2.PushAsync(new UserLoginPage(_LanguageService, _UserLoginVM));

                MainPage = nav2;
                break;
            case AccessMessage.LandingPage:
            case AccessMessage.AccessTokenExpired:
                MainPage = new NavigationPage(new LandingPage(_LanguageService, _AdminVM, _AdminLoginVM, _UserLoginVM));
                break;
        }
    }
    #endregion
}
