using Inventory.API.Models;

namespace Inventory.API.Repositories.CompanyRegistration;

public interface ICompanyRepository
{
    Task<RepoResult<Company>> GetCompanyById(int id);
    Task<RepoResult<int>> CreateCompany(Company company);
    Task<RepoResult<bool>> UpdateCompany(Company company);
    Task<RepoResult<bool>> DeleteCompany(Company company);
}
