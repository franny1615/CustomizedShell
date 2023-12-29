using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Components;
using Maui.Inventory.Pages;

namespace Maui.Inventory;

public class App : Application
{
    private readonly ILanguageService _LanguageService;
    private readonly AdminRegisterViewModel _AdminVM;
    private readonly AdminLoginViewModel _AdminLoginVM;
    private readonly AppViewModel _AppVM;

    public App(
        ILanguageService languageService,
        SplashViewModel splashViewModel,
        AdminRegisterViewModel adminVM,
        AdminLoginViewModel adminLoginVM,
        AppViewModel appVM)
    {
        _LanguageService = languageService;
        _AdminVM = adminVM;
        _AdminLoginVM = adminLoginVM;
        _AppVM = appVM;

        Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

        MainPage = new SplashPage(_LanguageService, splashViewModel);

        RegisterListeners();
    }

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
                MainPage = new AdminShell(_LanguageService);
                break;
            case AccessMessage.UserSignedIn:
                MainPage = new UserShell();
                break;
            case AccessMessage.AdminLogout:
                // TODO: go to admin login page
                break;
            case AccessMessage.UserLogout:
                // TODO: go to user login page
                break;
            case AccessMessage.LandingPage:
            case AccessMessage.AccessTokenExpired:
                MainPage = new NavigationPage(new LandingPage(_LanguageService, _AdminVM, _AdminLoginVM));
                break;
        }
    }
}
