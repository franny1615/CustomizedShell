using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
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
        string password,
        int editInventoryPermissions)
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
@editInvPerms INT      = {editInventoryPermissions},
@salt UNIQUEIDENTIFIER=NEWID(),
@success BIT = 0

BEGIN TRY
    INSERT INTO app_user 
    (
        UserName, 
        PasswordHash, 
        Salt, 
        AdminID,
        EditInventoryPermissions
    )
    VALUES
    (
        @username, 
        HASHBYTES('SHA2_512', @password+CAST(@salt AS NVARCHAR(36))), 
        @salt, 
        @adminId,
        @editInvPerms
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
        string password,
        string email,
        bool emailVerified, 
        int editInventoryPermissions)
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

        if (string.IsNullOrEmpty(email))
        {
            return new APIResponse<UserResponse>
            {
                Success = false,
                Data = UserResponse.NoEmail
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
    DATEADD(MONTH, 1, CURRENT_TIMESTAMP),
    'Trial License'
)

SET @insertedID = SCOPE_IDENTITY()

SELECT @insertedID;";

                var licenses = await SQLUtils.QueryAsync<int>(trialLicense);
                licenseId = licenses.First();
                #endregion

                #region INSERT NEW ADMIN QUERY
                int emailBit = emailVerified ? 1 : 0;
                string query = $@"
SET NOCOUNT ON

DECLARE 
@username      NVARCHAR(50)  = '{username}',
@password      NVARCHAR(50)  = '{password}',
@license       INT           = {licenseId},
@email         NVARCHAR(300) = '{email}',
@emailVerified BIT           = {emailBit},
@editInvPerms  INT           = {editInventoryPermissions},
@salt          UNIQUEIDENTIFIER=NEWID(),
@success       BIT = 0

BEGIN TRY
    INSERT INTO admin 
    (
        UserName, 
        PasswordHash, 
        Salt, 
        LicenseID,
        Email,
	    EmailVerified,
        EditInventoryPermissions
    )
    VALUES
    (
        @username, 
        HASHBYTES('SHA2_512', @password+CAST(@salt AS NVARCHAR(36))), 
        @salt, 
        @license,
        @email,
	    @emailVerified,
        @editInvPerms
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
    public async Task<APIResponse<Admin>> AuthenticateAdmin(
        string username,
        string password)
    {
        APIResponse<Admin> response = new();

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

SELECT 
    Id, 
    UserName, 
    LicenseID, 
    '' as AccessToken, 
    '' as Password,
    Email,
    EmailVerified,
    IsDarkModeOn,
    EditInventoryPermissions
FROM admin
WHERE admin.Id = @userID;";
            #endregion

            var authenticatedUser = (await SQLUtils.QueryAsync<Admin>(query)).First();
            if (authenticatedUser.Id != -1)
            {
                bool licenseValid = false;
                #region VALIDATE LICENSE
                string getExpirationDate = $@"
SELECT ExpirationDate
FROM license
INNER JOIN admin
ON admin.LicenseId = license.Id
WHERE admin.Id = {authenticatedUser.Id}";

                var expirationDatesResponse = await SQLUtils.QueryAsync<DateTime>(getExpirationDate);
                DateTime expirationDate = expirationDatesResponse.First();

                if (expirationDate > DateTime.Now)
                {
                    licenseValid = true; 
                }
                #endregion

                authenticatedUser.LicenseExpirationDate = expirationDate;
                authenticatedUser.IsLicenseValid = licenseValid;
                authenticatedUser.AccessToken = GenerateJWT(authenticatedUser.Id, username, authenticatedUser.Id, isAdminLogin: true);

                response.Success = true;
                response.Data = authenticatedUser;
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
    public async Task<APIResponse<User>> AuthenticateUser(
        int adminID,
        string username,
        string password)
    {
        APIResponse<User> response = new();

        try 
        {
            #region AUTHENTICATE QUERY
        string query =$@"
SET NOCOUNT ON

DECLARE 
@adminID INT = {adminID},
@userID INT = -1,
@username NVARCHAR(50) = '{username}',
@password NVARCHAR(50) = '{password}'

IF EXISTS (SELECT TOP 1 Id FROM app_user WHERE UserName = @username)
BEGIN
    SET @userID=
    (
        SELECT Id 
        FROM app_user 
        WHERE UserName = @username 
        AND PasswordHash = HASHBYTES('SHA2_512', @password+CAST(Salt AS NVARCHAR(36)))
        AND AdminID = @adminID
    )
END

SELECT 
    Id,
    UserName,
    AdminID,
    '' as Password,
    '' as AccessToken,
    IsDarkModeOn,
    EditInventoryPermissions
FROM app_user
WHERE app_user.Id = @userID;";
        #endregion

            var authenticatedUser = (await SQLUtils.QueryAsync<User>(query)).First();
            if (authenticatedUser.Id != -1)
            {
                bool licenseValid = false;
                #region VALIDATE LICENSE
                string getExpirationDate = $@"
DECLARE @adminId INT = -1

SET @adminId = (SELECT AdminId from app_user WHERE app_user.Id = {authenticatedUser.Id})

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

                authenticatedUser.IsLicenseValid = licenseValid;
                authenticatedUser.AccessToken = GenerateJWT(authenticatedUser.Id, username, authenticatedUser.AdminID, isAdminLogin: false);

                response.Success = true;
                response.Data = authenticatedUser;
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
    private string GenerateJWT(int id, string username, int adminId, bool isAdminLogin)
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
                new Claim("AdminID", $"{adminId}"),
                new Claim("UserName", username),
                new Claim("IsAdminLogin", isAdminLogin ? "1": "0")
            }),
            Expires = DateTime.UtcNow.AddDays(1),
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
    public async Task<APIResponse<PaginatedQueryResponse<User>>> GetUsersForAdmin(
        PaginatedRequest request, int adminId)
    {
        #region SEARCH
        string searchQuery = "";
        if (!string.IsNullOrEmpty(request.Search))
        {
            string[] words = request.Search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in words)
            {
                searchQuery += $"AND UserName LIKE '{item}%'";
            }
        }
        #endregion

        #region QUERY
        string query = $@"
SELECT 
    Id,
    UserName,
    AdminID,
    EditInventoryPermissions,
    '' as Password,
    '' as AccessToken
FROM app_user
WHERE app_user.AdminID = {adminId}
{searchQuery}";
        #endregion

        #region TOTAL
        string totalQuery = $@"
SELECT COUNT(*)
FROM ({query}) users";
        #endregion

        APIResponse<PaginatedQueryResponse<User>> response = new();
        try
        {
            response.Data = new();
            response.Data.Total = (await SQLUtils.QueryAsync<int>(totalQuery)).First();

            query += $@"
ORDER BY UserName
OFFSET {request.Page * request.ItemsPerPage} ROWS
FETCH NEXT {request.ItemsPerPage} ROWS ONLY";

            response.Data.Items = (await SQLUtils.QueryAsync<User>(query)).ToList();
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
    public async Task<APIResponse<bool>> EditUser(User user)
    {
        #region SANITY CHECKS
        if (user == null)
        {
            return new()
            {
                Success = false,
                Message = "No user"
            };
        }

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
        int darkModeOn = user.IsDarkModeOn ? 1 : 0;
        string query = $@"
SET NOCOUNT ON

DECLARE 
@id       INT          = {user.Id},
@username NVARCHAR(50) = '{user.UserName}',
@password NVARCHAR(50) = '{user.Password}',
@adminId  INT          = {user.AdminID},
@darkMode BIT          = '{darkModeOn}',
@editInvPerms  INT     = {user.EditInventoryPermissions},
@salt     UNIQUEIDENTIFIER = NEWID(),
@success  BIT = 0

BEGIN TRY
    UPDATE app_user
    SET
        UserName = @username,
        {updatingPS}
        AdminID = @adminId,
        IsDarkModeOn = @darkMode,
        EditInventoryPermissions = @editInvPerms
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

    #region BEGIN EMAIL REGISTRATION
    public async Task<APIResponse<bool>> BeginEmailVerification(string email)
    {
        string businessEmail = Env.String(EnvironmentConstant.BUS_EML);
        string businessEmailPass = Env.String(EnvironmentConstant.BUS_PASS);

        int generatedNumber = new Random().Next(1000, 9999);

        #region EMAIL BOILER PLATE
        MailMessage mail = new();
        mail.To.Add(email);
        mail.From = new MailAddress(businessEmail, "Verify Email", Encoding.UTF8);
        mail.Subject = "Verify Email";
        mail.SubjectEncoding = Encoding.UTF8;
        mail.Body = $"<h1>Your verification code is : <b>{generatedNumber}</b> </h1>";
        mail.BodyEncoding = Encoding.UTF8;
        mail.IsBodyHtml = true;
        mail.Priority = MailPriority.High;

        SmtpClient client = new();
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(businessEmail, businessEmailPass);
        client.EnableSsl = true;
        client.Port = 587;
        client.Host = "smtp.gmail.com";
        #endregion

        var response = new APIResponse<bool>();
        try
        {
            client.Send(mail);

            #region QUERY
            string query = $@"
            INSERT INTO email_validation
            (
                Email,
                Code
            )
            VALUES
            (
                '{email}',
                {generatedNumber}
            )";

            await SQLUtils.QueryAsync<object>(query);
            #endregion

            response.Data = true;
            response.Message = "";
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Data = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region VERIFY EMAIL
    public async Task<APIResponse<bool>> VerifyEmail(string email, int code)
    {
        var response = new APIResponse<bool>();
        try
        {
            #region QUERY
            string query = $@"
            SELECT Email, Code, Id 
            FROM email_validation
            WHERE Email = '{email}'
            AND Code = {code}";
            #endregion

            var emailValidation = (await SQLUtils.QueryAsync<EmailValidation>(query)).FirstOrDefault();

            if (emailValidation == null)
            {
                response.Success = false;
                response.Data = false;
                response.Message = $"ERROR >>> failed validation <<<";    
            }
            else
            {
                // it is temporary so clean it up,
                // keep table slim
                await SQLUtils.QueryAsync<object>($@"
                    DELETE FROM email_validation 
                    WHERE Id = {emailValidation.Id}");

                response.Success = true;
                response.Data = true;
                response.Message = "";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Data = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region EDIT ADMIN
    public async Task<APIResponse<bool>> EditAdmin(Admin admin)
    {
        #region SANITY CHECKS
        if (admin == null)
        {
            return new() { Success = false, Data = false, Message = "No admin provided" };
        }

        if (string.IsNullOrEmpty(admin.UserName))
        {
            return new() { Success = false, Data = false, Message = "No username." };
        }

        if (string.IsNullOrEmpty(admin.Email))
        {
            return new() { Success = false, Data = false, Message = "No email." };
        }
        #endregion

        #region PASSWORD UPDATE
        string updatingPS = "";
        if (!string.IsNullOrEmpty(admin.Password))
        {
            updatingPS = $@"
PasswordHash = HASHBYTES('SHA2_512', @password+CAST(@salt AS NVARCHAR(36))),
Salt = @salt,";
        }
        #endregion

        #region QUERY
        int emailBit = admin.EmailVerified ? 1 : 0;
        int darkModeBit = admin.IsDarkModeOn ? 1 : 0;
        string query = $@"
SET NOCOUNT ON

DECLARE 
@id            INT           = {admin.Id},
@username      NVARCHAR(50)  = '{admin.UserName}',
@password      NVARCHAR(50)  = '{admin.Password}',
@email         NVARCHAR(300) = '{admin.Email}',
@emailVerified BIT           = '{emailBit}',
@darkMode      BIT           = '{darkModeBit}',
@editInvPerms  INT            = {admin.EditInventoryPermissions},
@salt     UNIQUEIDENTIFIER = NEWID(),
@success  BIT = 0

BEGIN TRY
    UPDATE admin
    SET
        UserName = @username,
        {updatingPS}
        Email = @email,
        EmailVerified = @emailVerified,
        IsDarkModeOn = @darkMode,
        EditInventoryPermissions = @editInvPerms
    WHERE admin.Id = @id

    SET @success=1
END TRY
BEGIN CATCH
    SET @success=0 
END CATCH

select @success;";
        #endregion

        var response = new APIResponse<bool>();
        try
        {
            var successes = await SQLUtils.QueryAsync<int>(query);

            response.Success = successes.FirstOrDefault() == 1;
            response.Message = "Successfully updated admin";
            response.Data = successes.FirstOrDefault() == 1;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Data = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region DELETE ENTIRE ACCOUNT
    public async Task<APIResponse<bool>> DeleteEntireAccount(int adminId)
    {
        var response = new APIResponse<bool>();
        try
        {
            #region QUERY
            string sourceLicense = $@"
SELECT LicenseId FROM admin WHERE Id = {adminId}";
            int licenseId = (await SQLUtils.QueryAsync<int>(sourceLicense)).FirstOrDefault();

            string query = $@"
DELETE FROM app_user WHERE AdminID = {adminId}; 
DELETE FROM inventory_history WHERE AdminId = {adminId};
DELETE FROM inventory WHERE AdminId = {adminId};
DELETE FROM locations WHERE AdminId = {adminId};
DELETE FROM status WHERE AdminId = {adminId};
DELETE FROM quantity_types WHERE AdminId = {adminId};
DELETE FROM admin WHERE Id = {adminId};
DELETE FROM license WHERE Id = {licenseId};";
            #endregion

            await SQLUtils.QueryAsync<object>(query);
            response.Success = true;
            response.Data = true;
            response.Message = "";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Data = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region FEEDBACK
    public async Task<APIResponse<bool>> InsertUserFeedback(
        Feedback feedback, 
        int userId, 
        int adminId, 
        bool isAdmin)
    {
        var response = new APIResponse<bool>();
        try
        {
            #region QUERY
            int isAdminBit = isAdmin ? 1 : 0;
            string query = $@"
INSERT INTO feedback
(
    AdminId,
    UserId,
    WasAdmin,
    Subject,
    Body
)
VALUES
(
    {adminId},
    {userId},
    {isAdminBit},
    '{feedback.Subject}',
    '{feedback.Body}'
)";
            #endregion

            await SQLUtils.QueryAsync<object>(query);
            response.Success = true;
            response.Data = true;
            response.Message = "";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Data = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }
    #endregion

    #region GET FEEDBACK
    public async Task<APIResponse<PaginatedQueryResponse<Feedback>>> GetFeedback(PaginatedRequest request)
    {
        var response = new APIResponse<PaginatedQueryResponse<Feedback>>();
        try
        {
            #region QUERY
            string query = $@"
SELECT
    Id,
    AdminId,
    UserId,
    WasAdmin,
    Subject,
    Body,
    CreatedOn
FROM feedback";
            string totalQuery = $@"SELECT COUNT(*) FROM ({query}) feedback";
            string fullQuery = $@"
{query}
ORDER BY CreatedOn DESC
OFFSET {request.Page * request.ItemsPerPage} ROWS
FETCH NEXT {request.ItemsPerPage} ROWS ONLY";
            #endregion

            response.Data = new()
            {
                Total = (await SQLUtils.QueryAsync<int>(totalQuery)).First(),
                Items = (await SQLUtils.QueryAsync<Feedback>(fullQuery)).ToList(),
            };
            response.Success = true;
            response.Message = "";
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Data = new();
            response.Message = $"ERROR >>> {e.Message} <<<";
        }
        return response;
    }
    #endregion
}
