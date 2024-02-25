using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maui.Inventory.Api.Controllers;

[Route("api/inventory")]
public class InventoryController(
    IHttpContextAccessor httpContextAccessor,
    IInventoryRepository inventoryRepository) : BaseController
{
    [HttpGet]
    [Route("list")]
    [Authorize]
    public async Task<APIResponse<PaginatedQueryResponse<Models.Inventory>>> GetInventory(
        [FromQuery] PaginatedRequest paginatedRequest)
    {
        APIResponse<PaginatedQueryResponse<Models.Inventory>> response;

        try
        {
            var user = httpContextAccessor.HttpContext?.User!;
            int adminId = Env.GetAdminIDFromIdentity(user);

            response = await inventoryRepository.GetAll(paginatedRequest, adminId);
        }
        catch (Exception ex)
        {
            response = new();
            response.Success = false;
            response.Message = ex.Message;
            response.Data = new();
        }

        return response;
    }

    [HttpPost]
    [Route("insert")]
    [Authorize]
    public async Task<APIResponse<bool>> Insert([FromBody] Models.Inventory inventory)
    {
        return await inventoryRepository.Insert(inventory);
    }

    [HttpPost]
    [Route("update")]
    [Authorize]
    public async Task<APIResponse<bool>> Update([FromBody] Models.Inventory inventory)
    {
        return await inventoryRepository.Update(inventory);
    }

    [HttpPost]
    [Route("delete")]
    [Authorize]
    public async Task<APIResponse<bool>> Delete([FromBody] Models.Inventory inventory)
    {
        return await inventoryRepository.Delete(inventory);
    }

    [HttpGet]
    [Route("editPermissions")]
    [Authorize]
    public async Task<APIResponse<int>> GetPermissions()
    {
        var user = httpContextAccessor.HttpContext?.User!;
        int adminId = Env.GetAdminIDFromIdentity(user);
        int userId = Env.GetUserIdFromIdentity(user);
        bool isAdmin = Env.IsAdmin(user);

        return await inventoryRepository.GetPermissions(userId, adminId, isAdmin);
    }
}
