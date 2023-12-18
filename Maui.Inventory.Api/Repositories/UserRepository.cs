using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Maui.Inventory.Api.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace Maui.Inventory.Api.Repositories;

public class UserRepository : IUserRepository
{
    #region USER REGISTRATION
    public async Task<APIResponse<UserResponse>> RegisterUser(
        int adminId,
        string username,
        string password)
    {
        #region BASIC SANITY CHECKS
        if (string.IsNullOrEmpty(username))
        {
            return new APIResponse<UserResponse>
            {
                Success = false,
                Data = UserResponse.NoUsername
            };
        }

        if (string.IsNullOrEmpty(password))
        {
            return new APIResponse<UserResponse>
            {
                Success = false,
                Data = UserResponse.NoPassword
            };
        }
        #endregion

        APIResponse<UserResponse> response = new();
        try
        {
            Admin? admin = null;
            #region GET ADMIN
            string adminQuery = $@"
SELECT Id, UserName, LicenseID FROM admin WHERE Id = {adminId}";

            var adminQueryResponse = await SQLUtils.QueryAsync<Admin>(adminQuery);
            admin = adminQueryResponse.FirstOrDefault();
            #endregion

            bool exists = false;
            #region CHECK IF user ALREADY EXISTS QUERY
            string existsQuery = $@"
SELECT UserName 
FROM app_user  
WHERE UserName = '{username}'
AND app_user.AdminID = {adminId}";

            var usernames = await SQLUtils.QueryAsync<string>(existsQuery);
            exists = usernames.FirstOrDefault() != null;
            #endregion

            if (admin == null)
            {
                response.Success = false;
                response.Data = UserResponse.AdminDoesNotExist;
            }
            else if (exists)
            {
                response.Success = false;
                response.Data = UserResponse.AlreadyExists;
            }
            else
            {
                #region CREATE USER
                string query = $@"
SET NOCOUNT ON

DECLARE 
@username NVARCHAR(50) = '{username}',
@password NVARCHAR(50) = '{password}',
@adminId  INT          = {adminId},
@salt UNIQUEIDENTIFIER=NEWID(),
@success BIT = 0

BEGIN TRY
    INSERT INTO app_user 
    (
        UserName, 
        PasswordHash, 
        Salt, 
        AdminID
    )
    VALUES
    (
        @username, 
        HASHBYTES('SHA2_512', @password+CAST(@salt AS NVARCHAR(36))), 
        @salt, 
        @adminId
    )
    SET @success=1
END TRY
BEGIN CATCH
    SET @success=0 
END CATCH

select @success;";
                #endregion

                var successes = await SQLUtils.QueryAsync<int>(query);

                response.Success = successes.FirstOrDefault() == 1;
                response.Data = successes.FirstOrDefault() == 1 ? UserResponse.SuccessfullyRegistered : UserResponse.ServerError;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
            response.Data = UserResponse.ServerError;
        }

        return response;
    }
    #endregion

    #region ADMIN REGISTRATION
    public async Task<APIResponse<UserResponse>> RegisterAdmin(
        string username,
        string password)
    {
        #region BASIC SANITY CHECKS
        if (string.IsNullOrEmpty(username))
        {
            return new APIResponse<UserResponse>
            {
                Success = false,
                Data = UserResponse.NoUsername
            };
        }

        if (string.IsNullOrEmpty(password))
        {
            return new APIResponse<UserResponse>
            {
                Success = false,
                Data = UserResponse.NoPassword
            };
        }
        #endregion

        APIResponse<UserResponse> response = new();
        try
        {
            bool adminExists = false;
            #region CHECK IF admin ALREADY EXISTS QUERY
            string adminExistsQuery = $@"
SELECT UserName 
FROM admin 
WHERE UserName = '{username}'";

            var usernames = await SQLUtils.QueryAsync<string>(adminExistsQuery);
            adminExists = usernames.FirstOrDefault() != null;
            #endregion

            if (adminExists)
            {
                response.Success = false;
                response.Data = UserResponse.AlreadyExists;
            }
            else
            {
                int licenseId = -1;
                #region GENERATE TRIAL LICENSE
                string trialLicense = $@"
SET NOCOUNT ON

DECLARE @insertedID INT = -1

INSERT INTO license
(
    ExpirationDate,
    Description
)
VALUES
(
    DATEADD(MONTH, 3, CURRENT_TIMESTAMP),
    'Trial License'
)

SET @insertedID = SCOPE_IDENTITY()

SELECT @insertedID;";

                var licenses = await SQLUtils.QueryAsync<int>(trialLicense);
                licenseId = licenses.First();
                #endregion

                #region INSERT NEW ADMIN QUERY
                string query = $@"
SET NOCOUNT ON

DECLARE 
@username NVARCHAR(50) = '{username}',
@password NVARCHAR(50) = '{password}',
@license  INT          = {licenseId},
@salt UNIQUEIDENTIFIER=NEWID(),
@success BIT = 0

BEGIN TRY
    INSERT INTO admin 
    (
        UserName, 
        PasswordHash, 
        Salt, 
        LicenseID
    )
    VALUES
    (
        @username, 
        HASHBYTES('SHA2_512', @password+CAST(@salt AS NVARCHAR(36))), 
        @salt, 
        @license
    )
    SET @success=1
END TRY
BEGIN CATCH
    SET @success=0 
END CATCH

select @success;";
                #endregion

                var successes = await SQLUtils.QueryAsync<int>(query);

                response.Success = successes.FirstOrDefault() == 1;
                response.Data = successes.FirstOrDefault() == 1 ? UserResponse.SuccessfullyRegistered : UserResponse.ServerError;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
            response.Data = UserResponse.ServerError;
        }

        return response;
    }
    #endregion

    #region AUTH ADMIN
    public async Task<APIResponse<AuthenticatedUser>> AuthenticateAdmin(
        string username,
        string password)
    {
        APIResponse<AuthenticatedUser> response = new();

        try
        {
            #region AUTHENTICATE QUERY
            string query = $@"
SET NOCOUNT ON

DECLARE 
@userID INT = -1,
@username NVARCHAR(50) = '{username}',
@password NVARCHAR(50) = '{password}'

IF EXISTS (SELECT TOP 1 Id FROM admin WHERE UserName = @username)
BEGIN
    SET @userID=
    (
        SELECT Id 
        FROM admin 
        WHERE UserName=@username 
        AND PasswordHash=HASHBYTES('SHA2_512', @password+CAST(Salt AS NVARCHAR(36)))
    )
END

SELECT @userID;";
            #endregion

            int authenticatedUserID = (await SQLUtils.QueryAsync<int>(query)).FirstOrDefault();
            if (authenticatedUserID != -1)
            {
                bool licenseValid = false;
                #region VALIDATE LICENSE
                string getExpirationDate = $@"
SELECT ExpirationDate
FROM license
INNER JOIN admin
ON admin.LicenseId = license.Id
WHERE admin.Id = {authenticatedUserID}";

                var expirationDatesResponse = await SQLUtils.QueryAsync<DateTime>(getExpirationDate);
                DateTime expirationDate = expirationDatesResponse.First();

                if (expirationDate > DateTime.Now)
                {
                    licenseValid = true; 
                }
                #endregion

                response.Success = licenseValid;
                response.Data = new() { AccessToken = licenseValid ? GenerateJWT(authenticatedUserID, username) : "" };
            }
            else
            {
                response.Success = false;
                response.Message = "admin did not exist";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region AUTH USER
    public async Task<APIResponse<AuthenticatedUser>> AuthenticateUser(
        string username,
        string password)
    {
        APIResponse<AuthenticatedUser> response = new();

        try 
        {
            #region AUTHENTICATE QUERY
        string query =$@"
SET NOCOUNT ON

DECLARE 
@userID INT = -1,
@username NVARCHAR(50) = '{username}',
@password NVARCHAR(50) = '{password}'

IF EXISTS (SELECT TOP 1 Id FROM app_user WHERE UserName = @username)
BEGIN
    SET @userID=
    (
        SELECT Id 
        FROM app_user 
        WHERE UserName=@username 
        AND PasswordHash=HASHBYTES('SHA2_512', @password+CAST(Salt AS NVARCHAR(36)))
    )
END

SELECT @userID;";
        #endregion

            int authenticatedUserID = (await SQLUtils.QueryAsync<int>(query)).FirstOrDefault();
            if (authenticatedUserID != -1)
            {
                bool licenseValid = false;
                #region VALIDATE LICENSE
                string getExpirationDate = $@"
DECLARE @adminId INT = -1

SET @adminId = (SELECT AdminId from app_user WHERE app_user.Id = {authenticatedUserID})

SELECT ExpirationDate
FROM license
INNER JOIN admin
ON admin.LicenseId = license.Id
WHERE admin.Id = @adminId";

                var expirationDatesResponse = await SQLUtils.QueryAsync<DateTime>(getExpirationDate);
                DateTime expirationDate = expirationDatesResponse.First();

                if (expirationDate > DateTime.Now)
                {
                    licenseValid = true;
                }
                #endregion

                response.Success = licenseValid;
                response.Data = new() { AccessToken = licenseValid ? GenerateJWT(authenticatedUserID, username) : "" };
            }
            else
            {
                response.Success = false;
                response.Message = "user did not exist";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region HELPERS
    private string GenerateJWT(int id, string username)
    {
        string issuer = Env.String(EnvironmentConstant.JWT_ISS);
        var audience = Env.String(EnvironmentConstant.JWT_AUD);
        var key = Encoding.ASCII.GetBytes(Env.String(EnvironmentConstant.JWT_KEY));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserID", $"{id}"),
                new Claim("UserName", username)
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }
    #endregion

    #region GET USERS
    public async Task<APIResponse<PaginatedQueryResponse<UserRegistration>>> GetUsersForAdmin(
        UsersRequest request)
    {
        #region SEARCH
        string searchQuery = "";
        if (!string.IsNullOrEmpty(request.Quantities.Search))
        {
            string[] words = request.Quantities.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in words)
            {
                searchQuery += $"AND UserName LIKE '{item}%'";
            }
        }
        #endregion

        #region QUERY
        string query = $@"
SELECT Id, UserName, AdminID
FROM app_user
WHERE app_user.AdminID = {request.AdminId}
{searchQuery}";
        #endregion

        #region TOTAL
        string totalQuery = $@"
SELECT COUNT(*)
FROM ({query}) users";
        #endregion

        APIResponse<PaginatedQueryResponse<UserRegistration>> response = new();
        try
        {
            response.Data = new();
            response.Data.Total = (await SQLUtils.QueryAsync<int>(totalQuery)).First();

            query += $@"
ORDER BY UserName
OFFSET {request.Quantities.Page * request.Quantities.ItemsPerPage} ROWS
FETCH NEXT {request.Quantities.ItemsPerPage} ROWS ONLY";

            response.Data.Items = (await SQLUtils.QueryAsync<UserRegistration>(query)).ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region DELETE USER
    public async Task<APIResponse<bool>> DeleteUserForAdmin(
        int adminId,
        int userId)
    {
        #region QUERY
        string query = $@"
DELETE FROM app_user
WHERE app_user.Id = {userId}
AND app_user.AdminID = {adminId}";
        #endregion

        APIResponse<bool> response = new();
        try
        {
            await SQLUtils.QueryAsync<object>(query);

            response.Success = true;
            response.Message = $"Deleted user with id {userId}";
            response.Data = true;
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Data = false;
            response.Message = $"ERROR >>> {e.Message} <<<";
        }

        return response;
    }
    #endregion

    #region EDIT USER
    public async Task<APIResponse<bool>> EditUser(UserRegistration user)
    {
        #region SANITY CHECKS
        if (string.IsNullOrEmpty(user.UserName))
        {
            return new()
            {
                Success = false,
                Message = "no username."
            };
        }
        #endregion

        #region UPDATING PASSWORD
        string updatingPS = "";
        if (!string.IsNullOrEmpty(user.Password))
        {
            updatingPS = $@"
PasswordHash = HASHBYTES('SHA2_512', @password+CAST(@salt AS NVARCHAR(36))),
Salt = @salt,";
        }
        #endregion

        #region QUERY
        string query = $@"
SET NOCOUNT ON

DECLARE 
@id       INT          = {user.Id},
@username NVARCHAR(50) = '{user.UserName}',
@password NVARCHAR(50) = '{user.Password}',
@adminId  INT          = {user.AdminID},
@salt     UNIQUEIDENTIFIER = NEWID(),
@success  BIT = 0

BEGIN TRY
    UPDATE app_user
    SET
        UserName = @username,
        {updatingPS}
        AdminID = @adminId
    WHERE app_user.Id = @id
    AND   app_user.AdminID = @adminId

    SET @success=1
END TRY
BEGIN CATCH
    SET @success=0 
END CATCH

select @success;";
        #endregion

        APIResponse<bool> response = new();
        try
        {
            var successes = await SQLUtils.QueryAsync<int>(query);

            response.Success = successes.FirstOrDefault() == 1;
            response.Message = "Successfully updated user";
            response.Data = successes.FirstOrDefault() == 1;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion
}
