using Inventory.API.Models;

namespace Inventory.API.Repositories.CompanyRegistration;

public class CompanyRepository(ILogger<CompanyRepository> logger) : BaseRepository, ICompanyRepository
{
    public async Task<RepoResult<int>> CreateCompany(Company company)
    {
        RepoResult<int> result = new();
        string query = "";
        try
        {
            query = $@"
SET NOCOUNT ON

INSERT INTO company
(
    Name,
    Address1,
    Address2,
    Address3,
    Country,
    City,
    State,
    LicenseExpiresOn
)
VALUES
(
    '{company.Name}',
    '{company.Address1}',
    '{company.Address2}',
    '{company.Address3}',
    '{company.Country}',
    '{company.City}',
    '{company.State}',
    DATEADD(MONTH, 1, CURRENT_TIMESTAMP)
);

SELECT SCOPE_IDENTITY()";
            result.Data = (await QueryAsync<int>(query)).First();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, query);
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<bool>> DeleteCompany(Company company)
    {
        RepoResult<bool> result = new();
        string query = "";
        try
        {
            query = $@"DELETE FROM company WHERE company.Id = {company.Id}";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, query);
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<Company>> GetCompanyById(int id)
    {
        RepoResult<Company> result = new();
        string query = "";
        try
        {
            query = $@"
SELECT
    Id,
    Name,
    Address1,
    Address2,
    Address3,
    Country,
    City,
    State,
    LicenseExpiresOn
FROM company
WHERE company.Id = {id}";
            result.Data = (await QueryAsync<Company>(query)).First();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, query);
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<bool>> UpdateCompany(Company company)
    {
        RepoResult<bool> result = new();
        string query = "";
        try
        {
            string licenseUpdate = "";
            if (company.LicenseExpiresOn != null)
            {
                licenseUpdate = $"LicenseExpiresOn = CAST('{company.LicenseExpiresOn?.ToString("MM/dd/yyyy HH:mm:ss")}' as DATETIME),";
            }

            query = $@"
UPDATE company SET
    Name = '{company.Name}',
    Address1 = '{company.Address1}',
    Address2 = '{company.Address2}',
    Address3 = '{company.Address3}',
    Country = '{company.Country}',
    City = '{company.City}',
    {licenseUpdate}
    State = '{company.State}',
    Zip = '{company.Zip}'
WHERE company.Id = {company.Id}";
            await QueryAsync<object>(query);
            result.Data = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, query);
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }
}
