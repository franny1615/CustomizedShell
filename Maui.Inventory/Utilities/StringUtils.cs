using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Models.AdminModels;
using Maui.Inventory.Models.UserModels;
using Microsoft.AppCenter.Crashes;
using System.IdentityModel.Tokens.Jwt;

namespace Maui.Inventory.Utilities;

public static class StringUtils
{
    public static async Task<AccessMessage> IsAccessTokenValid(
        IDAL<User> userDAL, 
        IDAL<Admin> adminDAL)
    {
        User user = null;
        Admin admin = null;
        try
        {
            user = (await userDAL.GetAll()).First();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }

        try
        {
            admin = (await adminDAL.GetAll()).First();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }

        if (user is not null)
        {
            return IsJWTExpired(user.AccessToken) ? AccessMessage.UserSignedIn : AccessMessage.UserLogout;
        }
        else if (admin is not null)
        {
            return IsJWTExpired(admin.AccessToken) ? AccessMessage.AdminSignedIn : AccessMessage.AdminLogout;
        }

        return AccessMessage.AccessTokenExpired;
    }

    public static bool IsJWTExpired(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

        try
        {
            var expirationClaim = jsonToken.Claims.First(claim => claim.Type == "exp");
            var ticks = long.Parse(expirationClaim.Value);

            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;
            var now = DateTime.UtcNow;

            return tokenDate >= now;
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);

            return false;
        }
    }
}
