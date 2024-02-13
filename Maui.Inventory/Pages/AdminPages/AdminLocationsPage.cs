using Maui.Components;
using Maui.Components.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminLocationsPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    #endregion

    #region Constructor
    public AdminLocationsPage(ILanguageService languageService) : base(languageService)
    {
        _LangService = languageService;
        Title = _LangService.StringForKey("Locations");
    }
    #endregion
}
