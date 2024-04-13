using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Resources.Languages;
using System.Globalization;

namespace Inventory.MobileApp.Services;

public class LanguageService
{
    public static LanguageService Instance { get; } = new();

    private LanguageService()
    {
        Lang.Culture = CultureInfo.CurrentCulture;
    }

    public string this[string resourceKey]
    {
        get
        {
            try
            {
                string result = Lang.ResourceManager.GetString(resourceKey, Lang.Culture) ?? "";
                return !string.IsNullOrEmpty(result) ? result : resourceKey;
            }
            catch
            {
                return resourceKey;
            }
        }
    }

    public static void SetCulture(CultureInfo culture)
    {
        Lang.Culture = culture;
        WeakReferenceMessenger.Default.Send(new InternalMsg(InternalMessage.LanguageChanged));
    }
}
