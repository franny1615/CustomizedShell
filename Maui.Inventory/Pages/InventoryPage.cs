using Maui.Components;

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
		Content = new Grid();
	}
}