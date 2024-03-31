namespace ftim.mobile.Utilities;

public static class KeyUtil
{
    public static string CurrentTheme
    {
        get => Preferences.Get(Constants.ThemeKey, "light");
        set => Preferences.Set(Constants.ThemeKey, value);
    }
    public static string CurrentAuth 
    {
        get => Preferences.Get(Constants.AuthKey, "");
        set => Preferences.Set(Constants.AuthKey, value);
    }
}
