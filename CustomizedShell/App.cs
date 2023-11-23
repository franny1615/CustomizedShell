using CommunityToolkit.Mvvm.Messaging;
using CustomizedShell.Models;
using CustomizedShell.Pages;
using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Interfaces;

namespace CustomizedShell;

public class App : Application
{
    private readonly ILanguageService _LanguageService;
    private readonly IDAL<User> _UserDAL;
    private readonly LoginViewModel _LoginViewModel;

    public App(
        ILanguageService languageService, 
        IDAL<User> userDAL,
        LoginViewModel loginViewModel)
    {
        _LanguageService = languageService;
        _LoginViewModel = loginViewModel;

        Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

        MainPage = new SplashScreen(languageService, userDAL);

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
        if (message.Value == "signed-out")
        {
            MainPage = new NavigationPage(new LoginPage(_LanguageService, _LoginViewModel));
        }
        else if (message.Value == "signed-in")
        {
            MainPage = new AppShell(_LanguageService, _UserDAL);
        }
    }
}
