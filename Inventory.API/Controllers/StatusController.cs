using Inventory.API.Models;
using Inventory.API.Repositories;
using Inventory.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Route("api/status")]
public class StatusController(
    IHttpContextAccessor httpContextAccessor,
    ICrudRepository<Status> statusRepo) : BaseController(httpContextAccessor)
{
    [HttpGet]
    [Authorize]
    [Route("details")]
    [ProducesResponseType<Status>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDetails([FromQuery] int statusId)
    {
        var repoResult = await statusRepo.Get(statusId, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpGet]
    [Authorize]
    [Route("search")]
    [ProducesResponseType<SearchResult<Status>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchStatuses([FromQuery] SearchRequest request)
    {
        var repoResult = await statusRepo.Get(request, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpDelete]
    [Authorize]
    [Route("delete")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromQuery] int statusId)
    {
        var repoResult = await statusRepo.Delete(statusId, CompanyId);
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
    public async Task<IActionResult> Insert([FromBody] Status status)
    {
        var repoResult = await statusRepo.Insert(status, CompanyId);
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
    public async Task<IActionResult> Update([FromBody] Status status)
    {
        var repoResult = await statusRepo.Update(status, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }
}