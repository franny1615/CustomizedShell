using Maui.Components;
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
    private readonly TabBar _tabBar = new();
    #endregion

    #region Constructor
    public UserShell(ILanguageService languageService)
    {
        _LangService = languageService;

        _profile.Title = _LangService.StringForKey("Profile");

        _tabBar.Items.Add(_profile);
        Items.Add(_tabBar);
    }
    #endregion
}

