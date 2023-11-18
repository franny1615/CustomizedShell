using CustomizedShell.Resources.Localization;
using System.ComponentModel;
using System.Globalization;

namespace CustomizedShell.Services;

public class LanguageService : INotifyPropertyChanged
{
    public static LanguageService Instance { get; } = new();

    public string this[string resourceKey]
        => AppLanguage.ResourceManager.GetObject(resourceKey, AppLanguage.Culture).ToString();

    public event PropertyChangedEventHandler PropertyChanged;

    public void SetCulture(CultureInfo culture)
    {
        AppLanguage.Culture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}
