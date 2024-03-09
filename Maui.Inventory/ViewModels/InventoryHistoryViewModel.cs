using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels;

public partial class InventoryHistoryViewModel : ObservableObject, IMaterialListVM<Models.Inventory>
{
    private readonly IInventoryService _inventoryService;

    public int ItemsPerPage { get; set; } = 10;
    public MaterialPaginationModel PaginationModel { get; set; } = new();
    public MaterialEntryModel SearchModel { get; set; } = new();
    public ObservableCollection<Models.Inventory> Items { get; set; } = new();

    public int InventoryId { get; set; } = 0;

    public InventoryHistoryViewModel(
        ILanguageService languageService,
        IInventoryService inventoryService,
        int inventoryId)
    {
        _inventoryService = inventoryService;
        InventoryId = inventoryId;

        SearchModel.Placeholder = languageService.StringForKey("Search");
        SearchModel.PlaceholderIcon = MaterialIcon.Search;
        SearchModel.Keyboard = Keyboard.Plain;
        SearchModel.EntryStyle = EntryStyle.Search;
    }

    public async Task GetItems()
    {
        Items.Clear();

        var locations = await _inventoryService.GetHistory(new ListRequest
        {
            Page = PaginationModel.CurrentPage - 1,
            ItemsPerPage = ItemsPerPage,
            Search = SearchModel.Text
        }, InventoryId);

        for (int i = 0; i < locations.Items.Count; i++)
        {
            Items.Add(locations.Items[i]);
        }

        var addendum = locations.Total % ItemsPerPage == 0 ? 0 : 1;
        var division = locations.Total / ItemsPerPage;
        var calculatedTotal = (locations.Total % ItemsPerPage == 0) ? division : division + addendum;

        PaginationModel.TotalPages = locations.Total > ItemsPerPage ? calculatedTotal : 1;
    }
}
