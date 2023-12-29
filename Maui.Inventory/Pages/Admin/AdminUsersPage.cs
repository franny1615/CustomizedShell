using Maui.Components;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages.Admin;

public class AdminUsersPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    #endregion

    #region Constructor
    public AdminUsersPage(
        ILanguageService languageService) : base(languageService)
    {
        _LangService = languageService;

        Title = _LangService.StringForKey("Employees");
    }
    #endregion
}