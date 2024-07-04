﻿using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.ViewModels;

public class InventorySearchViewModel : ISearchViewModel<Models.Inventory>
{
    private const int PAGE_SIZE = 15;

    public List<Models.Inventory> Items { get; set; } = new List<Models.Inventory>();
    public int TotalPages { get; set; } = 1;
    public int Page { get; set; } = 0;

    public async Task Search(string search)
    {
        var parameters = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(search))
        {
            parameters.Add("Search", search);
        }
        parameters.Add("Page", Page.ToString());
        parameters.Add("PageSize", PAGE_SIZE.ToString());

        var response = await NetworkService.Get<SearchResult<Models.Inventory>>(Endpoints.searchInventory, parameters);

        if (response == null)
            response = new();

        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            // TODO: log the error to the cloud or something.
            return;
        }

        Items = response.Data?.Items ?? [];
        int total = response.Data?.Total ?? 0;
        int addendum = total % PAGE_SIZE == 0 ? 0 : 1;
        TotalPages = (total / PAGE_SIZE) + addendum;

        if (TotalPages == 0)
            TotalPages = 1;
    }

    public async Task<NetworkResponse<int>> InsertInventory(Models.Inventory inventory)
    {
        return await NetworkService.Post<int>(Endpoints.insertInventory, inventory);
    }

    public async Task<NetworkResponse<DeleteResult>> DeleteInventory(int id)
    {
        return await NetworkService.Delete<DeleteResult>(Endpoints.deleteInventory, new Dictionary<string, string>
        {
            { "inventoryId", id.ToString() }
        });
    }

    public async Task<NetworkResponse<bool>> UpdateInventory(Models.Inventory inventory)
    {
        return await NetworkService.Post<bool>(Endpoints.updateInventory, inventory);
    }
}
