using Inventory.API.Models;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class StatusSearchPage : BasePage
{
    private readonly StatusSearchViewModel _ViewModel;
    private readonly SearchView<Status> _Search;

    public StatusSearchPage(StatusSearchViewModel statusSearchViewModel)
    {
        _ViewModel = statusSearchViewModel;
        _Search = new(_ViewModel);
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 8 };
        _Search.CardTemplate = new DataTemplate(() =>
        {
            // TODO:
            return new Grid();
        });

        _Search.Padding = 12;

        Title = LanguageService.Instance["Statuses"];
        Content = _Search;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.TriggerRefresh();
    }
}
