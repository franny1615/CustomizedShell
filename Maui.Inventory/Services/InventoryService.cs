using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory.Services;

public class InventoryService(IAPIService apiService) : IInventoryService
{
    public async Task<ListNetworkResponse<Models.Inventory>> GetAll(ListRequest request)
    {
        var parameters = new Dictionary<string, string>()
        {
            { "Page", request.Page.ToString() },
            { "ItemsPerPage", request.ItemsPerPage.ToString() }
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            parameters.Add("Search", request.Search);
        }

        return await apiService.Get<ListNetworkResponse<Models.Inventory>>(Endpoint.GetInventory, parameters);
    }

    public async Task<ListNetworkResponse<Models.Inventory>> GetHistory(ListRequest request, int inventoryId)
    {
        var parameters = new Dictionary<string, string>()
        {
            { "Page", request.Page.ToString() },
            { "ItemsPerPage", request.ItemsPerPage.ToString() },
            { "InventoryId", inventoryId.ToString() }
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            parameters.Add("Search", request.Search);
        }

        return await apiService.Get<ListNetworkResponse<Models.Inventory>>(Endpoint.GetInventoryHistory, parameters);
    }

    public async Task<bool> Insert(Models.Inventory itemToInsert)
    {
        return await apiService.Post<bool>(Endpoint.InsertInventory, itemToInsert);
    }

    public async Task<bool> Update(Models.Inventory itemToUpdate)
    {
        return await Update(itemToUpdate, null);
    }

    public async Task<bool> Update(Models.Inventory itemToUpdate, Models.Inventory cachedItem)
    {
        return await apiService.Post<bool>(Endpoint.UpdateInventory, new
        {
            inventory = itemToUpdate,
            previousInventory = cachedItem
        });
    }

    public async Task<bool> Delete(Models.Inventory itemToDelete)
    {
        return await apiService.Post<bool>(Endpoint.DeleteInventory, itemToDelete);
    }

    public async Task<int> GetEditInventoryPermissions()
    {
        return await apiService.Get<int>(Endpoint.GetEditInvPerms, new());
    }
}
