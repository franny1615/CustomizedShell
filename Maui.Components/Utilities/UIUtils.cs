namespace Maui.Components.Utilities;

public static class UIUtils
{
    public static FontImageSource MaterialIconFIS(
        string icon,
        Color color,
        int size = 20)
    {
        return new()
        {
            Glyph = icon,
            FontFamily = MaterialIcon.FontName,
            Size = size,
            Color = color
        };
    }

    public static void ToggleDarkMode(bool darkModeEnabled)
    {
        if (darkModeEnabled)
        {
            Application.Current.Resources["PageColor"] = Application.Current.Resources["PageDark"];
            Application.Current.Resources["TextColor"] = Application.Current.Resources["TextDark"];
            Application.Current.Resources["PopupColor"] = Application.Current.Resources["PopupDark"];
            Application.Current.Resources["CardColor"] = Application.Current.Resources["CardDark"];
        }
        else
        {
            Application.Current.Resources["PageColor"] = Application.Current.Resources["PageLight"];
            Application.Current.Resources["TextColor"] = Application.Current.Resources["TextLight"];
            Application.Current.Resources["PopupColor"] = Application.Current.Resources["PopupLight"];
            Application.Current.Resources["CardColor"] = Application.Current.Resources["CardLight"];
        }
    }
}
