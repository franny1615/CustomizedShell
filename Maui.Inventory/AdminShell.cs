using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Inventory.Models;
using Maui.Inventory.Pages;
using Maui.Inventory.Pages.AdminPages;

namespace Maui.Inventory;

public class AdminShell : Shell
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private readonly ShellContent _dashboard = new()
    {
        ContentTemplate = new DataTemplate(typeof(AdminDashboardPage)),
        Icon = "list_ic.png",
    };
    private readonly ShellContent _inventory = new()
    {
        ContentTemplate = new DataTemplate(typeof(InventoryPage)),
        Icon = "package_ic.png",
    };
    private readonly ShellContent _users = new()
    {
        ContentTemplate = new DataTemplate(typeof(AdminUsersPage)),
        Icon = "users.png"
    };
    private readonly ShellContent _profile = new()
    {
        ContentTemplate = new DataTemplate(typeof(AdminProfilePage)),
        Icon = "user.png"
    };
    private readonly TabBar _tabBar = new();
    #endregion

    #region Constructor
    public AdminShell(ILanguageService languageService)
    {
        _LangService = languageService;

        _dashboard.Title = _LangService.StringForKey("Data");
        _inventory.Title = _LangService.StringForKey("Inventory");
        _users.Title = _LangService.StringForKey("Employees");
        _profile.Title = _LangService.StringForKey("Profile");

        _tabBar.Items.Add(_inventory);
        _tabBar.Items.Add(_dashboard);
        _tabBar.Items.Add(_users);
        _tabBar.Items.Add(_profile);
        Items.Add(_tabBar);

        Routing.RegisterRoute(nameof(AdminLocationsPage), typeof(AdminLocationsPage));
        Routing.RegisterRoute(nameof(AdminStatusesPage), typeof(AdminStatusesPage));
        Routing.RegisterRoute(nameof(AdminQuantityTypesPage), typeof(AdminQuantityTypesPage));
        Routing.RegisterRoute(nameof(SendFeedbackPage), typeof(SendFeedbackPage));
        Routing.RegisterRoute(nameof(AdminManageSubscriptionPage), typeof(AdminManageSubscriptionPage));

        WeakReferenceMessenger.Default.Register<InternalMessage>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => ProcessInternalMsg(msg.Value.ToString()));
        });
    }

    private void ProcessInternalMsg(string message)
    {
        if (message == "language-changed")
        {
            _dashboard.Title = _LangService.StringForKey("Data");
            _inventory.Title = _LangService.StringForKey("Inventory");
            _users.Title = _LangService.StringForKey("Employees");
            _profile.Title = _LangService.StringForKey("Profile");
        }
    }
    #endregion
}
