using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Inventory.Models;
using Maui.Inventory.Pages;
using Maui.Inventory.Pages.UserPages;

namespace Maui.Inventory;

public class UserShell : Shell
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private readonly ShellContent _profile = new()
    {
        ContentTemplate = new DataTemplate(typeof(UserProfilePage)),
        Icon = "users.png"
    };
    private readonly ShellContent _inventory = new()
    {
        ContentTemplate = new DataTemplate(typeof(InventoryPage)),
        Icon = "package_ic.png",
    };
    private readonly TabBar _tabBar = new();
    #endregion

    #region Constructor
    public UserShell(ILanguageService languageService)
    {
        _LangService = languageService;

        _profile.Title = _LangService.StringForKey("Profile");
        _inventory.Title = _LangService.StringForKey("Inventory");

        _tabBar.Items.Add(_inventory);
        _tabBar.Items.Add(_profile);
        Items.Add(_tabBar);

        Routing.RegisterRoute(nameof(SendFeedbackPage), typeof(SendFeedbackPage));

        WeakReferenceMessenger.Default.Register<InternalMessage>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => ProcessInternalMsg(msg.Value.ToString()));
        });
    }

    private void ProcessInternalMsg(string message)
    {
        if (message == "language-changed")
        {
            _inventory.Title = _LangService.StringForKey("Inventory");
            _profile.Title = _LangService.StringForKey("Profile");
        }
    }
    #endregion
}

