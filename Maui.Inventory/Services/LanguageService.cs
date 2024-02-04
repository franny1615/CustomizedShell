using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;
using Maui.Inventory.Resources.Localization;
using Microsoft.AppCenter.Crashes;
using System.Globalization;

namespace Maui.Inventory.Services;

public class LanguageService
{
    public static LanguageService Instance { get; } = new();

    public string this[string key]
    {
        get
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

    private LanguageService()
    {
        AppLanguage.Culture = CultureInfo.CurrentCulture;
    }

    public void SetCulture(CultureInfo culture)
    {
        AppLanguage.Culture = culture;
        WeakReferenceMessenger.Default.Send(new InternalMessage("app-language-changed"));
    }
}

