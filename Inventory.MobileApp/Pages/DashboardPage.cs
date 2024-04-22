using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.Pages;

public class DashboardPage : BasePage
{
	public DashboardPage()
	{
		Title = LanguageService.Instance["Dashboard"];

		ToolbarItems.Add(new ToolbarItem
		{
			IconImageSource = UIService.MaterialIcon(MaterialIcon.Dark_mode, 21, Colors.White),
			Command = new Command(() => UIService.DisplayThemeSwitcher(this))
		});
		ToolbarItems.Add(new ToolbarItem
		{
			IconImageSource = UIService.MaterialIcon(MaterialIcon.Language, 21, Colors.White),
			Command = new Command(() => UIService.DisplayLanguageSwitcher(this))
		});
		ToolbarItems.Add(new ToolbarItem
		{
			IconImageSource = UIService.MaterialIcon(MaterialIcon.Logout, 21, Colors.White),
			Command = new Command(SessionService.LogOut)
		});

		Content = new VerticalStackLayout();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		LanguageChanged += UpdateLanguageStrings;
    }

    protected override void OnDisappearing()
    {
		LanguageChanged -= UpdateLanguageStrings;
        base.OnDisappearing();
    }

	private void UpdateLanguageStrings(object? sender, EventArgs e)
	{
		Title = LanguageService.Instance["Dashboard"];
	}
}