using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class MainPage : BasePage
{
	#region Constructor
	public MainPage(ILanguageService languageService) : base(languageService)
	{
		Content = new Grid{ };
	}
	#endregion
}