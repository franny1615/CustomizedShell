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
	private readonly RefreshView _Refresh = new();
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
		_Refresh.Command = new Command(ReloadDashboard);
		BindableLayout.SetItemTemplate(_ContentLayout, new DataTemplate(() =>
		{
			var view = new DashboardTile();
			view.SetBinding(BindingContextProperty, ".");
			view.SetBinding(DashboardTile.CountProperty, "Count");
			view.SetBinding(DashboardTile.TitleProperty, "Name");
            view.Clicked += DashTileClicked;

			return view;
		}));
		
		_Refresh.Content = _ContentLayout;
		Content = _Refresh;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		LanguageChanged += UpdateLanguageStrings;
		_Refresh.IsRefreshing = true;
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

	private async void ReloadDashboard()
	{
		BindableLayout.SetItemsSource(_ContentLayout, new List<DashboardItem>());
		await _DashboardVM.LoadDashboard();
		BindableLayout.SetItemsSource(_ContentLayout, _DashboardVM.DashboardItems);
		_Refresh.IsRefreshing = false;
	}

    private void DashTileClicked(object? sender, EventArgs e)
    {
		if (sender is DashboardTile tile && tile.BindingContext is DashboardItem item)
		{
            switch (item.Type)
            {
                case DashboardItemType.Inventory:
                    break;
                case DashboardItemType.Employees:
                    break;
                case DashboardItemType.Statuses:
					Navigation.PushAsync(PageService.StatusSearch());
                    break;
                case DashboardItemType.Locations:
                    break;
                case DashboardItemType.QuantityTypes:
                    break;
                case DashboardItemType.Unknown:
                    break;
            }
        }
    }
}