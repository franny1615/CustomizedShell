using Inventory.Api.Models;

namespace Inventory.Api.Repositories.CompanyRegistration;

public interface ICompanyRepository
{
    Task<Company> GetCompanyById(int id);
    Task<int> CreateCompany(Company company);
    Task<bool> UpdateCompany(Company company);
    Task<bool> DeleteCompany(Company company);
}
