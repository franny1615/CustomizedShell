using System.Globalization;
using Inventory.MobileApp.Controls;

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

    public static ContentPage DisplayThemeSwitcher(this ContentPage page)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            string dark = LanguageService.Instance["Dark"];
            string light = LanguageService.Instance["Light"];

            string themeChoice = await page.DisplayActionSheet(
                LanguageService.Instance["Choose Theme"],
                null,
                null,
                [dark, light]);

            if (themeChoice == dark)
            {
                SessionService.CurrentTheme = "dark";
                ApplyTheme();
            }
            else if (themeChoice == light)
            {
                SessionService.CurrentTheme = "light";
                ApplyTheme();
            }
        });
        return page;
    }

    public static void ApplyTheme()
    {
        if (Application.Current == null)
        {
            return;
        }

        var app = Application.Current;
        var currentTheme = SessionService.CurrentTheme;
        if (currentTheme == "dark")
        {
            app.Resources["IndicatorColor"] = app.Resources["IndicatorColorDark"];
            app.Resources["TextColor"] = app.Resources["TextColorDark"];
            app.Resources["TextPlaceholderColor"] = app.Resources["TextPlaceholderColorDark"];
            app.Resources["DisabledTextColor"] = app.Resources["DisabledTextColorDark"];
            app.Resources["PageColor"] = app.Resources["PageColorDark"];
        }
        else if (currentTheme == "light")
        {
            app.Resources["IndicatorColor"] = app.Resources["IndicatorColorLight"];
            app.Resources["TextColor"] = app.Resources["TextColorLight"];
            app.Resources["TextPlaceholderColor"] = app.Resources["TextPlaceholderColorLight"];
            app.Resources["DisabledTextColor"] = app.Resources["DisabledTextColorLight"];
            app.Resources["PageColor"] = app.Resources["PageColorLight"];
        }
    }

    public static ContentPage DisplayCommonError(this ContentPage page, string errorMessage)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool share = await page.DisplayAlert(
                LanguageService.Instance["Error"],
                LanguageService.Instance["ErrorMsg"],
                LanguageService.Instance["Share"],
                LanguageService.Instance["Cancel"]);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(errorMessage);
#endif
            // TODO: api call to send error message over via API
        });
        return page;
    }

    public static Editor AutoSize(this Editor editor)
    {
        editor.AutoSize = EditorAutoSizeOption.TextChanges;
        return editor;
    }

    public static HorizontalStackLayout Spacing(this HorizontalStackLayout layout, int spacing)
    {
        layout.Spacing = spacing;
        return layout;
    }

    public static Border BorderColor(this Border border, Color color)
    {
        border.Stroke = color;
        return border;
    }

    public static Border BorderShape(this Border border, IShape? shape)
    {
        border.StrokeShape = shape;
        return border;
    }

    public static MaterialEntry Placeholder(this MaterialEntry entry, string placeholder)
    {
        entry.Placeholder = placeholder;
        return entry;
    }

    public static MaterialEntry PlaceholderIcon(this MaterialEntry entry, string icon)
    {
        entry.PlaceholderIcon = icon;
        return entry;
    }
}
