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

            if (admin == null)
            {
                response.Success = false;
                response.Data = UserResponse.AdminDoesNotExist;
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
}
