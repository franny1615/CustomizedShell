using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maui.Inventory.Api.Controllers;

[Route("api/quantityTypes")]
public class QuantityTypesController(
    IHttpContextAccessor httpContextAccessor,
    IQuantityTypeRepository quantityTypeRepo): BaseController
{
    [HttpGet]
    [Route("list")]
    [Authorize]
    public async Task<APIResponse<PaginatedQueryResponse<QuantityType>>> GetAll(
        [FromQuery] PaginatedRequest paginatedRequest)
    {
        APIResponse<PaginatedQueryResponse<QuantityType>> response;

        try
        {
            var user = httpContextAccessor.HttpContext?.User!;
            int adminId = Env.GetAdminIDFromIdentity(user);

            response = await quantityTypeRepo.GetAll(paginatedRequest, adminId);
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
    public async Task<APIResponse<bool>> Insert([FromBody] QuantityType inventory)
    {
        return await quantityTypeRepo.Insert(inventory);
    }

    [HttpPost]
    [Route("update")]
    [Authorize]
    public async Task<APIResponse<bool>> Update([FromBody] QuantityType inventory)
    {
        return await quantityTypeRepo.Update(inventory);
    }

    [HttpPost]
    [Route("delete")]
    [Authorize]
    public async Task<APIResponse<bool>> Delete([FromBody] QuantityType inventory)
    {
        return await quantityTypeRepo.Delete(inventory);
    }
}
