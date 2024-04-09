using Inventory.Api.Models;

namespace Inventory.Api.Repositories.CompanyRegistration;

public class CompanyRepository(ILogger<CompanyRepository> logger) : BaseRepository, ICompanyRepository
{
    public async Task<int> CreateCompany(Company company)
    {
        int newId = -1;
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
            newId = (await QueryAsync<int>(query)).First();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, query);
        }
        return newId;
    }

    public async Task<bool> DeleteCompany(Company company)
    {
        bool result = false;
        string query = "";
        try
        {
            query = $@"DELETE FROM company WHERE company.Id = {company.Id}";
            await QueryAsync<object>(query);
            result = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, query);
        }
        return result;
    }

    public async Task<Company> GetCompanyById(int id)
    {
        Company company;
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
            company = (await QueryAsync<Company>(query)).First();
        }
        catch (Exception ex)
        {
            company = new();
            logger.LogError(ex, query);
        }
        return company;
    }

    public async Task<bool> UpdateCompany(Company company)
    {
        var result = false;
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
    State = '{company.State}'
WHERE company.Id = {company.Id}";
            await QueryAsync<object>(query);
            result = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, query);
        }
        return result;
    }
}
