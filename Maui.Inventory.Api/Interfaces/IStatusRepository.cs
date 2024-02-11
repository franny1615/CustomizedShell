using Maui.Inventory.Api.Models;

namespace Maui.Inventory.Api.Interfaces;

public interface IStatusRepository
{
    public Task<APIResponse<bool>> Insert(Status status);
    public Task<APIResponse<bool>> Delete(Status status);
    public Task<APIResponse<bool>> Update(Status status);
    public Task<APIResponse<PaginatedQueryResponse<Status>>> GetAll(
        PaginatedRequest request,
        int adminId);
}
