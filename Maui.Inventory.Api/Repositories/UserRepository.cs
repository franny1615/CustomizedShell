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
    public async Task<bool> RegisterNewUser(
        string username,
        string password,
        bool isAdmin)
    {
        int isAdminActual = isAdmin ? 1 : 0;
        int success = 0;

        try
        {
            #region CHECK IF USER ALREADY EXISTS QUERY
        string userExistsQuery = $@"
SELECT UserName 
FROM user_table 
WHERE UserName = '{username}'";
        #endregion

            var un = await SQLUtils.QueryAsync<string>(userExistsQuery);
            bool alreadyExisted = !string.IsNullOrEmpty(un.FirstOrDefault());

            #region INSERT NEW USER QUERY
        string query =$@"
SET NOCOUNT ON

DECLARE 
@username NVARCHAR(50) = '{username}',
@password NVARCHAR(50) = '{password}',
@isAdmin BIT = {isAdminActual},
@salt UNIQUEIDENTIFIER=NEWID(),
@success BIT = 0

BEGIN TRY
    INSERT INTO user_table 
    (
        UserName, 
        PasswordHash, 
        Salt, 
        IsAdmin
    )
    VALUES
    (
        @username, 
        HASHBYTES('SHA2_512', @password+CAST(@salt AS NVARCHAR(36))), 
        @salt, 
        @isAdmin
    )
    SET @success=1
END TRY
BEGIN CATCH
    SET @success=0 
END CATCH

select @success;";
        #endregion

            bool shouldInsertNewUser = !alreadyExisted && !string.IsNullOrEmpty(password);
            if (shouldInsertNewUser)
            {
                var successes = await SQLUtils.QueryAsync<int>(query);
                success = successes.FirstOrDefault();
            }
        }
        catch 
        {
            success = 0;
        }

        return success == 1;
    }

    public async Task<AuthenticatedUser> AuthenticateUser(
        string username,
        string password)
    {
        AuthenticatedUser result = new();

        try 
        {
            #region AUTHENTICATE QUERY
        string query =$@"
SET NOCOUNT ON

DECLARE 
@userID INT = -1,
@username NVARCHAR(50) = '{username}',
@password NVARCHAR(50) = '{password}'

IF EXISTS (SELECT TOP 1 Id FROM user_table WHERE UserName = @username)
BEGIN
    SET @userID=
    (
        SELECT Id 
        FROM user_table 
        WHERE UserName=@username 
        AND PasswordHash=HASHBYTES('SHA2_512', @password+CAST(Salt AS NVARCHAR(36)))
    )
END

SELECT @userID;";
        #endregion

            int authenticatedUserID = (await SQLUtils.QueryAsync<int>(query)).FirstOrDefault();
            if (authenticatedUserID != -1)
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
                        new Claim("UserID", $"{authenticatedUserID}"),
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

                result.AccessToken = jwtToken;
            }
        }
        catch (Exception ex)
        {
            #if DEBUG
            System.Diagnostics.Debug.WriteLine(ex);
            #endif
        }

        return result;
    }

    public async Task<bool> AdminCheck()
    {
        #region QUERY
        string query = $@"SELECT 1 FROM user_table WHERE IsAdmin = 1;";
        #endregion

        var queryResult = (await SQLUtils.QueryAsync<int>(query)).ToList();

        return queryResult.Count > 0;
    }
}
