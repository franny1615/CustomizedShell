using Maui.Components;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages;

public class InventoryPage : BasePage
{
	#region Private Properties
	private readonly ILanguageService _languageService;
	#endregion

	public InventoryPage(ILanguageService languageService) : base(languageService)
	{
		_languageService = languageService;

		Title = _languageService.StringForKey("Inventory");
		Content = new Grid();
	}
}