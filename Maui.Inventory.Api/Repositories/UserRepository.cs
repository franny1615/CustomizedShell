using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Utilities;

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

        return success == 1;
    }
}
