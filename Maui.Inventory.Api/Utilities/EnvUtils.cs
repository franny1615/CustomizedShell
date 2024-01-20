using System.Security.Claims;

namespace Maui.Inventory.Api;

public static class Env 
{
    public static string String(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? "";
    }

    public static int GetAdminIDFromIdentity(ClaimsPrincipal user)
    {
        string value = user.Claims.First((claim) => claim.Type == "AdminID").Value;
        return int.Parse(value);
    }
}
