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
    private readonly AppViewModel _AppVM;

    public App(
        ILanguageService languageService,
        SplashViewModel splashViewModel,
        AdminRegisterViewModel adminVM,
        AppViewModel appVM)
    {
        _LanguageService = languageService;
        _AdminVM = adminVM;
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

    private async void AccessControl(AccessMessage access)
    {
        switch (access)
        {
            case AccessMessage.SignedIn:
                if (await _AppVM.AdminSignedIn())
                {
                    MainPage = new AdminShell();
                }
                else if (await _AppVM.UserSignedIn())
                {
                    // TODO: user shell
                }
                break;
            case AccessMessage.FirstTimeLogin:
                MainPage = new NavigationPage(new LandingPage(_LanguageService, _AdminVM));
                break;
            case AccessMessage.LoggedOut:
            case AccessMessage.AccessTokenExpired:
                break;
        }
    }
}
