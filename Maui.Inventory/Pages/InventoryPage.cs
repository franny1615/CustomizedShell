using Maui.Components;
using Maui.Inventory.Components.Pages;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Maui.Inventory.Pages;

public class InventoryPage : ContentPage
{
	#region Private Properties
	private readonly ILanguageService _languageService;
	#endregion

	public InventoryPage(ILanguageService languageService)
	{
		_languageService = languageService;

		Title = _languageService.StringForKey("Inventory");
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
}