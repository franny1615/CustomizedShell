using Maui.Inventory.Api.Models;

namespace Maui.Inventory.Api.Interfaces;

public interface IInventoryRepository
{
    public Task<APIResponse<bool>> Insert(Models.Inventory inventory);
    public Task<APIResponse<bool>> Delete(Models.Inventory inventory);
    public Task<APIResponse<bool>> Update(Models.Inventory inventory);
    public Task<APIResponse<PaginatedQueryResponse<Models.Inventory>>> GetAll(
        PaginatedRequest request,
        int adminId);
}
