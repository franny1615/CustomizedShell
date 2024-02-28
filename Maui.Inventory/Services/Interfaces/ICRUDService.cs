using Maui.Inventory.Models;

namespace Maui.Inventory.Services.Interfaces;

public interface ICRUDService<T>
{
    public Task<bool> Insert(T itemToInsert);
    public Task<bool> Delete(T itemToDelete);
    public Task<bool> Update(T itemToUpdate);
    public Task<ListNetworkResponse<T>> GetAll(ListRequest request);
}
