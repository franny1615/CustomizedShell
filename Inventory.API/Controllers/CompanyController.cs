using Inventory.Api.Models;
using Inventory.Api.Repositories.CompanyRegistration;
using Inventory.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[Route("api/company")]
public class CompanyController(
    ICompanyRepository companyRepository, 
    IHttpContextAccessor httpContextAccessor) : BaseController(httpContextAccessor)
{
    [HttpPost]
    [Route("register")]
    public async Task<int> Register([FromBody] Company company)
    {
        return await companyRepository.CreateCompany(company);
    }

    [HttpGet]
    [Route("details")]
    [Authorize]
    public async Task<Company> GetCompanyById()
    {
        return await companyRepository.GetCompanyById(UserId);
    }

    [HttpPost]
    [Route("update")]
    [Authorize]
    public async Task<bool> Update([FromBody] Company company)
    {
        return await companyRepository.UpdateCompany(company);
    }

    [HttpPost]
    [Route("delete")]
    [Authorize]
    public async Task<bool> Delete([FromBody] Company company)
    {
        return await companyRepository.DeleteCompany(company);
    }
}
