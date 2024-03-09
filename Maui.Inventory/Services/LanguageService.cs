using Maui.Inventory.Resources.Localization;
using Maui.Components;
using Microsoft.AppCenter.Crashes;
using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;

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

    public static void SetCulture(CultureInfo info)
    {
        AppLanguage.Culture = info;
        WeakReferenceMessenger.Default.Send(new InternalMessage("language-changed"));
    }

    public static void CheckLanguage()
    {
        string language = Preferences.Get(Constants.Language, "");
        if (string.IsNullOrEmpty(language))
        {
            Preferences.Set(Constants.Language, "English");
            SetCulture(new CultureInfo("en-US", false));
        }
        else
        {
            if (language == "English")
            {
                SetCulture(new CultureInfo("en-US", false));
            }
            else if (language == "Español")
            {
                SetCulture(new CultureInfo("es-ES", false));
            }
        }
    }
}
