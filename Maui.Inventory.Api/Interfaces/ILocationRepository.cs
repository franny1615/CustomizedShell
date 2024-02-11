using Maui.Inventory.Api.Models;

namespace Maui.Inventory.Api.Interfaces;

public interface ILocationRepository
{
    public Task<APIResponse<bool>> Insert(Location location);
    public Task<APIResponse<bool>> Delete(Location location);
    public Task<APIResponse<bool>> Update(Location location);
    public Task<APIResponse<PaginatedQueryResponse<Location>>> GetAll(
        PaginatedRequest request,
        int adminId);
}
