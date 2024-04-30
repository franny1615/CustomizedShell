using Inventory.API.Models;
using Inventory.API.Repositories.CompanyRegistration;
using Inventory.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Route("api/company")]
public class CompanyController(
    ICompanyRepository companyRepository, 
    IHttpContextAccessor httpContextAccessor) : BaseController(httpContextAccessor)
{
    [HttpPost]
    [Route("register")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] Company company)
    {
        var repoResult = await companyRepository.CreateCompany(company);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpGet]
    [Route("details")]
    [Authorize]
    [ProducesResponseType<Company>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCompanyById()
    {
        var repoResult = await companyRepository.GetCompanyById(CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpPost]
    [Route("update")]
    [Authorize]
    [ProducesResponseType<Company>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody] Company company)
    {
        var repoResult = await companyRepository.UpdateCompany(company);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpPost]
    [Route("delete")]
    [Authorize]
    [ProducesResponseType<Company>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromBody] Company company)
    {
        var repoResult = await companyRepository.DeleteCompany(company);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }
}
