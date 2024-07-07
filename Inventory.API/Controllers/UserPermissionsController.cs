using Inventory.API.Models;
using Inventory.API.Repositories;
using Inventory.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Route("api/permissions")]
public class UserPermissionsController(
    IHttpContextAccessor httpContextAccessor,
    IPermissionsRepository permissionsRepo) : BaseController (httpContextAccessor)
{
    [HttpGet]
    [Authorize]
    [Route("details")]
    [ProducesResponseType<UserPermissions>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDetails([FromQuery] int permId)
    {
        var repoResult = await permissionsRepo.Get(permId, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpGet]
    [Authorize]
    [Route("detailsByUser")]
    [ProducesResponseType<UserPermissions>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDetailsByUser([FromQuery] int userId)
    {
        var repoResult = await permissionsRepo.GetByUser(userId, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpGet]
    [Authorize]
    [Route("search")]
    [ProducesResponseType<SearchResult<UserPermissions>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchStatuses([FromQuery] SearchRequest request)
    {
        var repoResult = await permissionsRepo.Get(request, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpDelete]
    [Authorize]
    [Route("delete")]
    [ProducesResponseType<DeleteResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromQuery] int permId)
    {
        var repoResult = await permissionsRepo.Delete(permId, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpPost]
    [Authorize]
    [Route("insert")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Insert([FromBody] UserPermissions qtyType)
    {
        var repoResult = await permissionsRepo.Insert(qtyType, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpPost]
    [Authorize]
    [Route("update")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody] UserPermissions qtyType)
    {
        var repoResult = await permissionsRepo.Update(qtyType, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }
}
