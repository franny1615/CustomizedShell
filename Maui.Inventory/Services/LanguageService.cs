using Maui.Inventory.Resources.Localization;
using Maui.Components;

namespace Maui.Inventory.Services;

public class LanguageService : ILanguageService
{
    public string StringForKey(string key) => AppLanguage.ResourceManager.GetObject(key, AppLanguage.Culture).ToString();
}
