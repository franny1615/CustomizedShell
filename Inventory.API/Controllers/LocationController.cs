using Inventory.API.Models;
using Inventory.API.Repositories;
using Inventory.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Route("api/location")]
public class LocationController(
    IHttpContextAccessor httpContextAccessor,
    ICrudRepository<Location> locationRepo
) : BaseController(httpContextAccessor)
{
    [HttpGet]
    [Authorize]
    [Route("details")]
    [ProducesResponseType<Location>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDetails([FromQuery] int statusId)
    {
        var repoResult = await locationRepo.Get(statusId, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpGet]
    [Authorize]
    [Route("search")]
    [ProducesResponseType<SearchResult<Location>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchStatuses([FromQuery] SearchRequest request)
    {
        var repoResult = await locationRepo.Get(request, CompanyId);
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
        var repoResult = await locationRepo.Delete(statusId, CompanyId);
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
    public async Task<IActionResult> Insert([FromBody] Location location)
    {
        var repoResult = await locationRepo.Insert(location, CompanyId);
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
    public async Task<IActionResult> Update([FromBody] Location location)
    {
        var repoResult = await locationRepo.Update(location, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }
}