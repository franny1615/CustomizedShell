using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Components;
using Maui.Components.Interfaces;

namespace Maui.Inventory;

public class App : Application
{
    private readonly ILanguageService _LanguageService;
    private readonly LoginViewModel _LoginViewModel;

    public App(
        ILanguageService languageService, 
        IDAL<ApiUrl> apiDAL,
        LoginViewModel loginViewModel)
    {
        _LanguageService = languageService;
        _LoginViewModel = loginViewModel;

        Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

        // TODO: main page equal to loadin page

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
            case AccessMessage.SignedIn:
                // TODO: assign main page to user app shell
                break;
            case AccessMessage.LoggedOut:
            case AccessMessage.AccessTokenExpired:
                break;
        }
    }
}
