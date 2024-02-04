using Maui.Components.Pages;
using Maui.Inventory.Services;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Maui.Inventory.Components.Pages;

namespace Maui.Inventory.Pages;

public class ProfilePage : BasePage
{
    #region Constructor
    public ProfilePage() : base(LanguageService.Instance["Back"])
    {
        Title = LanguageService.Instance["Profile"];
        Content = new BlazorWebView
        {
            HostPage = "wwwroot/index.html",
            RootComponents =
            {
                new RootComponent
                {
                    Selector = "#app",
                    ComponentType = typeof(ProfileRazorPage)
                }
            }
        };
    }
    #endregion
}
