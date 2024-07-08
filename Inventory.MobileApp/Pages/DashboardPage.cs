using CommunityToolkit.Maui.Alerts;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;
using Microsoft.Maui.Layouts;

namespace Inventory.MobileApp.Pages;

public class DashboardPage : BasePage
{
	private readonly DashboardViewModel _DashboardVM;
	private readonly FlexLayout _ContentLayout = new()
	{
		Direction = FlexDirection.Row,
		JustifyContent = FlexJustify.Center,
		Wrap = FlexWrap.Wrap,
        Padding = new Thickness(0, 0, 0, 0),
		Margin = new Thickness(0, 0, 0, 0),
    };

	public DashboardPage(DashboardViewModel dashboardViewModel)
	{
		_DashboardVM = dashboardViewModel;

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
		BindableLayout.SetItemTemplate(_ContentLayout, new DataTemplate(() =>
		{
			var view = new DashboardTile();
			view.SetBinding(BindingContextProperty, ".");
			view.SetBinding(DashboardTile.TitleProperty, "Name");
			view.SetBinding(DashboardTile.TypeProperty, "Type");
            view.Clicked += DashTileClicked;

			return view;
		}));
		
		Content = new ScrollView
		{
			Orientation = ScrollOrientation.Vertical,
			Content = _ContentLayout
		};
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		LanguageChanged += UpdateLanguageStrings;
        _ = _DashboardVM.LoadProfile();
        ReloadDashboard();
    }

    protected override void OnDisappearing()
    {
		LanguageChanged -= UpdateLanguageStrings;
        base.OnDisappearing();
    }

	private void UpdateLanguageStrings(object? sender, EventArgs e)
	{
		Title = LanguageService.Instance["Dashboard"];
		_DashboardVM.UpdateDashboardNames();
	}

	private void ReloadDashboard()
	{
		BindableLayout.SetItemsSource(_ContentLayout, new List<DashboardItem>());
		_DashboardVM.LoadDashboard();
		BindableLayout.SetItemsSource(_ContentLayout, _DashboardVM.DashboardItems);
	}

    private void DashTileClicked(object? sender, EventArgs e)
    {
		if (sender is DashboardTile tile && tile.BindingContext is DashboardItem item)
		{
            switch (item.Type)
            {
                case DashboardItemType.Inventory:
					Navigation.PushAsync(PageService.InventorySearch());
                    break;
                case DashboardItemType.Employees:
					// TODO: 
                    break;
                case DashboardItemType.Statuses:
					Navigation.PushAsync(PageService.StatusSearch());
                    break;
                case DashboardItemType.Locations:
					Navigation.PushAsync(PageService.LocationSearch());
                    break;
                case DashboardItemType.QuantityTypes:
					Navigation.PushAsync(PageService.QuantityTypesSearch());
                    break;
				case DashboardItemType.Profile:
					Navigation.PushAsync(PageService.Profile());
					break;
                case DashboardItemType.Unknown:
                    break;
            }
        }
    }
}