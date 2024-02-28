using Maui.Inventory.Models;

namespace Maui.Inventory.Services.Interfaces;

public interface IInventoryService : ICRUDService<Models.Inventory>
{
    public Task<int> GetEditInventoryPermissions();
    public Task<bool> Update(Models.Inventory itemToUpdate, Models.Inventory cachedInventory);
    public Task<ListNetworkResponse<Models.Inventory>> GetHistory(ListRequest request, int inventoryId);
}
