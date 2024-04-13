using System.Globalization;

namespace Inventory.MobileApp.Services;

public static class UIService
{
    public static Image ApplyMaterialIcon(this Image image, string icon, float size, Color color)
    {
        image.Source = new FontImageSource
        {
            FontFamily = nameof(MaterialIcon),
            Glyph = icon,
            Size = size,
            Color = color
        };
        return image;
    }

    public static ContentPage DisplayLanguageSwitcher(this ContentPage page)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            string languageChoice = await page.DisplayActionSheet(
                LanguageService.Instance["Choose Language"],
                null,
                null,
                ["English", "Español"]);

            switch (languageChoice)
            {
                case "English":
                    SessionService.CurrentLanguageCulture = "en-US";
                    LanguageService.SetCulture(new CultureInfo("en-US"));
                    break;
                case "Español":
                    SessionService.CurrentLanguageCulture = "en-US";
                    LanguageService.SetCulture(new CultureInfo("es-ES"));
                    break;
            }
        });
        return page;
    }
}
