using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Layouts;

namespace Inventory.MobileApp.Pages;

public class DashboardPage : BasePage
{
	private readonly ScrollView _Scroll = new();
	private readonly FlexLayout _ContentLayout = new()
	{
		Direction = FlexDirection.Row,
		JustifyContent = FlexJustify.Center,
		Wrap = FlexWrap.Wrap,
	};
	private readonly DashboardTile _InventoryTile = new();
	private readonly DashboardTile _EmployeeTile = new();
	private readonly DashboardTile _StatusTile = new();
	private readonly DashboardTile _LocationsTile = new();
	private readonly DashboardTile _BarcodesTile = new();
	private readonly DashboardTile _QuantityTypesTile = new();	

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

		_InventoryTile.Title = LanguageService.Instance["Inventory"];
		_EmployeeTile.Title = LanguageService.Instance["Employees"];
		_StatusTile.Title = LanguageService.Instance["Statuses"];
		_BarcodesTile.Title = LanguageService.Instance["Barcodes"];
		_LocationsTile.Title = LanguageService.Instance["Locations"];
		_QuantityTypesTile.Title = LanguageService.Instance["Qty Types"];

		_ContentLayout.Add(_InventoryTile);
		_ContentLayout.Add(_EmployeeTile);
		_ContentLayout.Add(_StatusTile);
		_ContentLayout.Add(_BarcodesTile);
		_ContentLayout.Add(_LocationsTile);
		_ContentLayout.Add(_QuantityTypesTile);

		_Scroll.Content = _ContentLayout;
		Content = _Scroll;
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