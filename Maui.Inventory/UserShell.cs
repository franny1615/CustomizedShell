using Maui.Inventory.Pages;
using Maui.Inventory.Services;

namespace Maui.Inventory;

public class UserShell : Shell
{
    #region Constructor
    public UserShell()
    {
        Items.Add(new TabBar
        {
            Items = 
            {
                new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(ProfilePage)),
                    Title = LanguageService.Instance["Profile"],
                    Icon = "users.png",
                }
            }
        });
    }
    #endregion
}

