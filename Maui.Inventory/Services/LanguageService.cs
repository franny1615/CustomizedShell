using Maui.Inventory.Resources.Localization;
using Maui.Components;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory.Services;

public class LanguageService : ILanguageService
{
    public string StringForKey(string key)
    {
        try
        {
            string result = AppLanguage.ResourceManager.GetString(key, AppLanguage.Culture);
            return result;
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
            return key;
        }
    }
}
