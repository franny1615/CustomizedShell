using Maui.Components.Pages;
using Maui.Inventory.Components.Pages;
using Maui.Inventory.Services;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Maui.Inventory.Pages;

public class LandingPage : BasePage
{
    #region Constructor
    public LandingPage() : base(LanguageService.Instance["Back"])
    {
        NavigationPage.SetHasNavigationBar(this, false);
        Content = new BlazorWebView
        {
            HostPage = "wwwroot/index.html",
            RootComponents =
            {
                new RootComponent
                {
                    Selector = "#app",
                    ComponentType = typeof(LandingRazorPage)
                }
            }
        };
    }
    #endregion
}