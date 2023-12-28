using Maui.Inventory.Pages.Admin;

namespace Maui.Inventory;

public class AdminShell : Shell
{
    #region Private Properties
    private readonly ShellContent _dashboard = new()
    {
        ContentTemplate = new DataTemplate(typeof(AdminDashboardPage))  
    };
    private readonly TabBar _tabBar = new();
    #endregion

    #region Constructor
    public AdminShell()
    {
        _tabBar.Items.Add(_dashboard);
        Items.Add(_tabBar);
    }
    #endregion
}
