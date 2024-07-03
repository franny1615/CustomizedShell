using Inventory.API.Models;
using Inventory.API.Repositories;
using Inventory.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Route("api/quantityType")]
public class QuantityTypesController(
    IHttpContextAccessor httpContextAccessor,
    ICrudRepository<QuantityType> qtyTypeRepo) : BaseController(httpContextAccessor)
{
    [HttpGet]
    [Authorize]
    [Route("details")]
    [ProducesResponseType<QuantityType>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDetails([FromQuery] int qtyId)
    {
        var repoResult = await qtyTypeRepo.Get(qtyId, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpGet]
    [Authorize]
    [Route("search")]
    [ProducesResponseType<SearchResult<QuantityType>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchStatuses([FromQuery] SearchRequest request)
    {
        var repoResult = await qtyTypeRepo.Get(request, CompanyId);
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
    public async Task<IActionResult> Delete([FromQuery] int qtyId)
    {
        var repoResult = await qtyTypeRepo.Delete(qtyId, CompanyId);
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
    public async Task<IActionResult> Insert([FromBody] QuantityType qtyType)
    {
        var repoResult = await qtyTypeRepo.Insert(qtyType, CompanyId);
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
    public async Task<IActionResult> Update([FromBody] QuantityType qtyType)
    {
        var repoResult = await qtyTypeRepo.Update(qtyType, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }
}