using Maui.Inventory.Pages;
using Maui.Inventory.Services;

namespace Maui.Inventory;

public class AdminShell : Shell
{
    #region Constructor
    public AdminShell()
    {
        Items.Add(new TabBar
        {
            Items =
            {
                new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(InventoryPage)),
                    Title = LanguageService.Instance["Inventory"],
                    Icon = "package_ic.png"
                },
                new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(ProfilePage)),
                    Title = LanguageService.Instance["Profile"],
                    Icon = "user.png"
                }
            }
        });
    }
    #endregion
}
