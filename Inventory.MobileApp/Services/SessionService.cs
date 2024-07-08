using System.IdentityModel.Tokens.Jwt;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;

namespace Inventory.MobileApp.Services;

public static class SessionService
{
    public static string AuthToken 
    {
        get => Preferences.Get("kAuthToken", "");
        set => Preferences.Set("kAuthToken", value);
    }

    public static string APIUrl
    {
        get => Preferences.Get("kAPIURL", "");
        set => Preferences.Set("kAPIURL", value);
    }

    public static string CurrentLanguageCulture
    {
        get => Preferences.Get("kCulture", "en-US");
        set => Preferences.Set("kCulture", value);
    }

    public static string CurrentTheme
    {
        get => Preferences.Get("kTheme", "light");
        set => Preferences.Set("kTheme", value);
    }

    public static bool IsFirstInstall
    {
        get => Preferences.Get("kFirstInstall", "1") == "1";
        set => Preferences.Set("kFirstInstall", value ? "1": "0");
    }

    public static User CurrentUser { get; set; } = new User();
    public static UserPermissions CurrentPermissions { get; set; } = new UserPermissions();

    public static void LogOut()
    {
        AuthToken = "";
        WeakReferenceMessenger.Default.Send(new InternalMsg(InternalMessage.LoggedOut));
    }

    public static bool IsAuthValid()
    {
        if (string.IsNullOrEmpty(AuthToken))
        {
            return false;
        }
        return CheckTokenIsValid(AuthToken);
    }

    public static bool CheckTokenIsValid(string token)
    {
        var tokenTicks = GetTokenExpirationTime(token);
        var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;
        var now = DateTime.Now.ToUniversalTime();
        var valid = tokenDate >= now;
        return valid;
    }

    public static long GetTokenExpirationTime(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
        var ticks= long.Parse(tokenExp);
        return ticks;
    }
}
