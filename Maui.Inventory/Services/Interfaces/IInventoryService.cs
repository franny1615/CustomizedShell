namespace Maui.Inventory.Services.Interfaces;

public interface IInventoryService : ICRUDService<Models.Inventory>
{
    public Task<int> GetEditInventoryPermissions();
}
