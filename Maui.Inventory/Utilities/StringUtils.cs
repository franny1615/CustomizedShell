using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Microsoft.AppCenter.Crashes;

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
            return Components.Utilities.StringUtils.IsJWTExpired(user.AccessToken) ? AccessMessage.UserSignedIn : AccessMessage.UserLogout;
        }
        else if (admin is not null)
        {
            return Components.Utilities.StringUtils.IsJWTExpired(admin.AccessToken) ? AccessMessage.AdminSignedIn : AccessMessage.AdminLogout;
        }

        return AccessMessage.AccessTokenExpired;
    }
}
