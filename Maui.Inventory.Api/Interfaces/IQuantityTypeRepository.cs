using Maui.Inventory.Api.Models;

namespace Maui.Inventory.Api.Interfaces;

public interface IQuantityTypeRepository
{
    public Task<APIResponse<bool>> Insert(QuantityType quantityType);
    public Task<APIResponse<bool>> Delete(QuantityType quantityType);
    public Task<APIResponse<bool>> Update(QuantityType quantityType);
    public Task<APIResponse<PaginatedQueryResponse<QuantityType>>> GetAll(
        PaginatedRequest request,
        int adminId);
}
