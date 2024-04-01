using System.Globalization;
using ftim.mobile.Models;

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
    public static string CurrentLanguage
    {
        get => Preferences.Get(Constants.LanguageKey, "en");
        set
        {
            try 
            {
                Preferences.Set(Constants.LanguageKey, value);
                Language.Instance.SetCulture(new CultureInfo(value));
            } 
            catch (Exception ex) 
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }   
        }
    }
}
