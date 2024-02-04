using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Inventory.Pages;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory;

public class App : Application
{
    #region Private Properties
    private readonly AppViewModel _AppVM;
    #endregion

    #region Constructor
    public App(SplashViewModel splashViewModel, AppViewModel appVM)
    {
        _AppVM = appVM;

        Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

        MainPage = new SplashPage(splashViewModel);

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
            MainPage = new NavigationPage(new LandingPage());
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

    private void AccessControl(AccessMessage access)
    {
        switch (access)
        {
            case AccessMessage.AdminSignedIn:
                MainPage = new AdminShell();
                break;
            case AccessMessage.UserSignedIn:
                MainPage = new UserShell();
                break;
            case AccessMessage.AdminLogout:
                // TODO: bring this back
                break;
            case AccessMessage.UserLogout:
                // TODO: bring this back
                break;
            case AccessMessage.LandingPage:
            case AccessMessage.AccessTokenExpired:
                MainPage = new NavigationPage(new LandingPage());
                break;
        }
    }
    #endregion
}
