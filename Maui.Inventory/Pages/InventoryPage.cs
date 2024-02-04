using Maui.Components.Pages;
using Maui.Inventory.Components.Pages;
using Maui.Inventory.Services;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Maui.Inventory.Pages;

public class InventoryPage : BasePage
{
    #region Constructor
    public InventoryPage() : base(LanguageService.Instance["Back"])
	{
		Title = LanguageService.Instance["Inventory"];
		Content = new BlazorWebView
		{
			HostPage = "wwwroot/index.html",
			RootComponents = 
			{
				new RootComponent
				{
					Selector = "#app",
					ComponentType = typeof(InventoryRazorPage)
				}
			}
		};
	}
    #endregion
}