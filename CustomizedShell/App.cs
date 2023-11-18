using CommunityToolkit.Mvvm.Messaging;
using CustomizedShell.Models;
using CustomizedShell.Pages;
using CustomizedShell.ViewModels;

namespace CustomizedShell;

public class App : Application
{
    private readonly LoginViewModel _LoginViewModel;

    public App(LoginViewModel loginViewModel)
    {
        _LoginViewModel = loginViewModel;

        Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

        MainPage = new SplashScreen();

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
            MainPage = new NavigationPage(new LoginPage(_LoginViewModel));
        }
        else if (message.Value == "signed-in")
        {
            MainPage = new AppShell();
        }
    }
}
