using Maui.Components;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages.Admin;

public class AdminProfilePage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    #endregion

    #region Constructor
    public AdminProfilePage(
        ILanguageService languageService) : base(languageService)
    {
        _LangService = languageService;

        Title = _LangService.StringForKey("Profile");
    }
    #endregion
}