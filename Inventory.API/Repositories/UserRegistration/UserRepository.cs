using Inventory.API.Models;
using Inventory.API.Utilities;

namespace Inventory.API.Repositories.UserRegistration;

public class UserRepository(ILogger<UserRepository> logger) : BaseRepository, IUserRepository
{
    public async Task<RepoResult<string>> AuthenticateUser(User user)
    {
        RepoResult<string> result = new();  
        try
        {
            string authQuery = $@"
SET NOCOUNT ON

DECLARE @userID INT = -1

IF EXISTS (SELECT TOP 1 Id FROM app_user WHERE UserName = '{user.UserName}')
BEGIN
    SET @userID=
    (
        SELECT Id 
        FROM app_user 
        WHERE UserName='{user.UserName}' 
        AND PasswordHash=HASHBYTES('SHA2_512', '{user.Password}'+CAST(Salt AS NVARCHAR(36)))
    )
END

SELECT 
    Id, 
    CompanyID, 
    UserName, 
    '' as Password,
    IsDarkModeOn,
    Localization,
    Email,
    PhoneNumber,
    IsCompanyOwner
FROM app_user
WHERE app_user.Id = @userID;";
            var authedUser = (await QueryAsync<User>(authQuery)).FirstOrDefault();
            result.Data = authedUser != null ? Auth.MinJWTForUser(authedUser) : "";
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "Error occurred during auth");
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<int>> CreateUser(User user)
    {
        RepoResult<int> result = new();
        try
        {
            var existsCheck = await DoesUserNameExist(user.UserName);
            if (!string.IsNullOrEmpty(existsCheck.ErrorMessage))
            {
                result.ErrorMessage = existsCheck.ErrorMessage;
            }
            else if (existsCheck.Data)
            {
                result.Data = -1;
            }
            else
            {
                string insertQuery = $@"
SET NOCOUNT ON

DECLARE 
@salt    UNIQUEIDENTIFIER=NEWID(),
@success INT = -1 

BEGIN TRY
    INSERT INTO app_user 
    (
        CompanyID, 
        UserName,
        PasswordHash,
        Salt, 
        IsDarkModeOn,
        Localization,
	    Email,
        PhoneNumber,
        IsCompanyOwner
    )
    VALUES
    (
        {user.CompanyID},
        '{user.UserName}',
        HASHBYTES('SHA2_512', '{user.Password}'+CAST(@salt AS NVARCHAR(36))), 
        @salt, 
        {(user.IsDarkModeOn ? 1 : 0)},
        '{user.Localization}',
        '{user.Email}',
	    '{user.PhoneNumber}',
        {(user.IsCompanyOwner ? 1 : 0)}
    )
    SET @success=SCOPE_IDENTITY()
END TRY
BEGIN CATCH
    SET @success=-1
END CATCH

select @success;";
                result.Data = (await QueryAsync<int>(insertQuery)).First();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during user creation");
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<bool>> DeleteUser(User user)
    {
        RepoResult<bool> result = new();
        string deleteQuery = "";
        try
        {
            deleteQuery = $@"DELETE FROM app_user WHERE app_user.Id = {user.Id}";
            await QueryAsync<object>(deleteQuery);
            result.Data = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, deleteQuery);
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<bool>> DoesUserNameExist(string userName)
    {
        RepoResult<bool> result = new();    
        string checkQuery = "";
        try
        {
            checkQuery = $@"SELECT 999 FROM app_user WHERE app_user.UserName = '{userName}';";
            int exists = (await QueryAsync<int>(checkQuery)).FirstOrDefault();
            result.Data = exists == 999;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
            logger.LogError(ex, checkQuery);
        }
        return result;
    }

    public async Task<RepoResult<bool>> UpdateUser(User user)
    {
        RepoResult<bool> result = new();    
        try
        {
            var currentDetails = await GetUserById(user.Id);
            if (!string.IsNullOrEmpty(currentDetails.ErrorMessage))
            {
                result.ErrorMessage = currentDetails.ErrorMessage;
            }

            bool usernameUpdated = true;
            if (currentDetails.Data?.UserName != user.UserName) 
            {
                var userAlreadyExists = await DoesUserNameExist(user.UserName);
                if (!string.IsNullOrEmpty(userAlreadyExists.ErrorMessage))
                {
                    result.ErrorMessage += $"\n\n{userAlreadyExists.ErrorMessage}";   
                }
                else if (userAlreadyExists.Data)
                {
                    usernameUpdated = false;
                }
            }

            bool updatedPassword = user.Password.Length == 0;
            if (!updatedPassword)
            {
                string updatePasswordQuery = $@"
SET NOCOUNT ON

DECLARE
@salt     UNIQUEIDENTIFIER = NEWID(),
@success  BIT = 0

BEGIN TRY
    UPDATE app_user SET
        PasswordHash = HASHBYTES('SHA2_512', '{user.Password}'+CAST(@salt AS NVARCHAR(36))),
        Salt = @salt
    WHERE app_user.Id = {user.Id}

    SET @success = 1
END TRY
BEGIN CATCH
    SET @success = 0
END CATCH

select @success;";
                updatedPassword = (await QueryAsync<int>(updatePasswordQuery)).First() == 1;
            }

            string updateQuery = $@"
UPDATE app_user SET 
    UserName = '{user.UserName}',
    IsDarkModeOn = {(user.IsDarkModeOn ? 1 : 0)},
    Localization = '{user.Localization}',
    Email = '{user.Email}',
    PhoneNumber = '{user.PhoneNumber}'
WHERE app_user.Id = {user.Id}";
            await QueryAsync<object>(updateQuery);
            result.Data = usernameUpdated && updatedPassword;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred on user update");

            result.ErrorMessage = !string.IsNullOrEmpty(result.ErrorMessage) ?
                $"{result.ErrorMessage}\n\n{ex}" :
                ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<User>> GetUserById(int id)
    {
        RepoResult<User> result = new();
        try
        {
            string query = $@"
SELECT
    Id,
    CompanyId,
    Username,
    '' as Password,
    IsDarkModeOn,
    Localization,
    Email,
    PhoneNumber,
    IsCompanyOwner
FROM app_user
WHERE app_user.Id = {id}";
            result.Data = (await QueryAsync<User>(query)).First();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred on user by id");
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }

    public async Task<RepoResult<SearchResult<User>>> Get(SearchRequest request, int companyId)
    {
        var result = new RepoResult<SearchResult<User>>();
        try
        {
            string query = $@"
declare 
@companyId int = {companyId},
@search NVARCHAR(max) = '{request.Search}',
@page int = {request.Page},
@pageSize int = {request.PageSize};
select 
    Id,
    CompanyId,
    Username,
    '' as Password,
    IsDarkModeOn,
    Localization,
    Email,
    PhoneNumber,
    IsCompanyOwner
from app_user 
where CompanyId = @companyId
and [Description] LIKE @search+'%'
order by Id desc 
offset (@page * @pageSize) rows 
fetch next @pageSize rows only";
            var items = (await QueryAsync<User>(query)).ToList();
            string totalQuery = $@"select COUNT(*) from location;";
            var total = (await QueryAsync<int>(totalQuery)).First();

            result.Data = new()
            {
                Items = items,
                Total = total
            };
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.ToString();
        }
        return result;
    }
}
