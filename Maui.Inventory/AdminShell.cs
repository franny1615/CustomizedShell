using Maui.Components;
using Maui.Inventory.Pages.Admin;

namespace Maui.Inventory;

public class AdminShell : Shell
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private readonly ShellContent _dashboard = new()
    {
        ContentTemplate = new DataTemplate(typeof(AdminDashboardPage)),
        Icon = "home.png",
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

        _dashboard.Title = _LangService.StringForKey("Dashboard");
        _users.Title = _LangService.StringForKey("Employees");
        _profile.Title = _LangService.StringForKey("Profile");

        _tabBar.Items.Add(_dashboard);
        _tabBar.Items.Add(_users);
        _tabBar.Items.Add(_profile);
        Items.Add(_tabBar);
    }
    #endregion
}
