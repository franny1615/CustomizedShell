using Maui.Components;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages.UserPages;

public class UserProfilePage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    #endregion

    #region Constructor
    public UserProfilePage(
        ILanguageService languageService) : base(languageService)
    {
        _LanguageService = languageService;
    }
    #endregion
}
