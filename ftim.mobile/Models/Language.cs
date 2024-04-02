using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using ftim.mobile.Resources.Languages;
using ftim.mobile.Utilities;

namespace ftim.mobile.Models;

public class Language
{
    public static Language[] Supported = [
        new Language { Description = "English", Abbreviation = "en" },
        new Language { Description = "Español", Abbreviation = "es" }, 
    ];

    public string Description { get; set; }
    public string Abbreviation { get; set; }

    public static Language Instance { get; } = new();
    public string this[string resourceKey]
    {
        get 
        {
            try 
            {
                var culture = new CultureInfo(KeyUtil.CurrentLanguage);
                string result = AppLanguage.ResourceManager.GetString(resourceKey, culture) ?? "";
                return string.IsNullOrEmpty(result) ? resourceKey : result;
            }
            catch 
            {
                return resourceKey;
            }
        }
    }
    
    public void SetCulture(CultureInfo culture) {
        AppLanguage.Culture = culture;
        WeakReferenceMessenger.Default.Send(new InternalMessage("language-changed"));
    }
}
