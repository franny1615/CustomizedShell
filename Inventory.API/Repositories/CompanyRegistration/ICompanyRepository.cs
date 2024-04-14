using Inventory.Api.Models;
using Inventory.API.Models;

namespace Inventory.Api.Repositories.CompanyRegistration;

public interface ICompanyRepository
{
    Task<RepoResult<Company>> GetCompanyById(int id);
    Task<RepoResult<int>> CreateCompany(Company company);
    Task<RepoResult<bool>> UpdateCompany(Company company);
    Task<RepoResult<bool>> DeleteCompany(Company company);
}
