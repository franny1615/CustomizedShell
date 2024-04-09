using Inventory.Api.Models;
using Inventory.Api.Repositories.CompanyRegistration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController(ICompanyRepository companyRepository)
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
    public async Task<Company> GetCompanyById([FromQuery] int companyId)
    {
        return await companyRepository.GetCompanyById(companyId);
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
