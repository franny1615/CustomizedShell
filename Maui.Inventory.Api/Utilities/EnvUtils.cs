namespace Maui.Inventory.Api;

public static class Env 
{
    public static string String(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? "";
    }
}
