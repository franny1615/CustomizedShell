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
}
