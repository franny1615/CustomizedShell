using Inventory.MobileApp.Services;

namespace Inventory.MobileApp;

public partial class App : Application
{
	public App()
	{
		Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

		MainPage = new NavigationPage(PageService.Landing());

		// TODO: on resume/on app start bring back user current language choice
	}
}
