using CustomizedShell.Resources.Localization;
using Maui.Components;

namespace CustomizedShell.Services;

public class LanguageService : ILanguageService
{
    public string StringForKey(string key) => AppLanguage.ResourceManager.GetObject(key, AppLanguage.Culture).ToString();
}
