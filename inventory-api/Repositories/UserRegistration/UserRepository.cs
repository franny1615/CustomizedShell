using inventory_api.Models;
using inventory_api.Utilities;

namespace inventory_api.Repositories.UserRegistration;

public class UserRepository(ILogger<UserRepository> logger) : BaseRepository, IUserRepository
{
    public async Task<string> AuthenticateUser(User user)
    {
        string jwt = "";
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
            var authedUser = (await QueryAsync<User>(authQuery)).First();
            jwt = Auth.MinJWTForUser(authedUser);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "Error occurred during auth");
        }
        return jwt;
    }

    public async Task<int> CreateUser(User user)
    {
        var insertedId = -1;
        try
        {
            bool doesUserNameExist = await DoesUserNameExist(user.UserName);
            if (doesUserNameExist)
            {
                return insertedId;
            }

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
            insertedId = (await QueryAsync<int>(insertQuery)).First();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during user creation");
        }

        return insertedId;
    }

    public async Task<bool> DeleteUser(User user)
    {
        bool result = false;
        string deleteQuery = "";
        try
        {
            deleteQuery = $@"DELETE FROM app_user WHERE app_user.Id = {user.Id}";
            await QueryAsync<object>(deleteQuery);
            result = true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, deleteQuery);
        }
        return result;
    }

    public async Task<bool> DoesUserNameExist(string userName)
    {
        bool result = false;
        string checkQuery = "";
        try
        {
            checkQuery = $@"SELECT 999 FROM app_user WHERE app_user.UserName = '{userName}';";
            int exists = (await QueryAsync<int>(checkQuery)).First();
            result = exists == 999;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, checkQuery);
        }
        return result;
    }

    public async Task<bool> UpdateUser(User user)
    {
        bool result = false;
        try
        {
            var currentDetails = await GetUserById(user.Id);
            if (currentDetails.UserName != user.UserName && (await DoesUserNameExist(user.UserName))) // has to be unique new username
            {
                return false;
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
            result = updatedPassword;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred on user update");
        }
        return result;
    }

    public async Task<User> GetUserById(int id)
    {
        User user;
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
            user = (await QueryAsync<User>(query)).First();
        }
        catch (Exception ex)
        {
            user = new();
            logger.LogError(ex, "Error occurred on user by id");
        }
        return user;
    }
}
