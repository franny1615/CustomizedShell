using Maui.Components;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminDashboardPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    #endregion

    #region Constructor
    public AdminDashboardPage(ILanguageService languageService) : base(languageService)
    {
        Shell.SetTabBarIsVisible(this, true);

        _LangService = languageService;

        Title = _LangService.StringForKey("Dashboard");
    }
    #endregion
}