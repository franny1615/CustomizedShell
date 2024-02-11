using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maui.Inventory.Api.Controllers;

[Route("api/status")]
public class StatusController(
    IHttpContextAccessor httpContextAccessor,
    IStatusRepository statusRepository) : BaseController
{
    [HttpGet]
    [Route("list")]
    [Authorize]
    public async Task<APIResponse<PaginatedQueryResponse<Status>>> GetStatusList(
        [FromQuery] PaginatedRequest request)
    {
        APIResponse<PaginatedQueryResponse<Status>> response;

        try
        {
            var user = httpContextAccessor.HttpContext?.User!;
            int adminId = Env.GetAdminIDFromIdentity(user);

            response = await statusRepository.GetAll(request, adminId);
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
    public async Task<APIResponse<bool>> Insert([FromBody] Status status)
    {
        return await statusRepository.Insert(status);
    }

    [HttpPost]
    [Route("update")]
    [Authorize]
    public async Task<APIResponse<bool>> Update([FromBody] Status status)
    {
        return await statusRepository.Update(status);
    }

    [HttpPost]
    [Route("delete")]
    [Authorize]
    public async Task<APIResponse<bool>> Delete([FromBody] Status status)
    {
        return await statusRepository.Delete(status);
    }
}
