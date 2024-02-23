using CommunityToolkit.Maui.Behaviors;

namespace Maui.Components.Pages;

public class BasePage(ILanguageService languageService) : ContentPage
{
    private readonly StatusBarBehavior _StatusBarBehavior = new();
    
    public readonly ILanguageService LanguageService = languageService;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        HideSoftInputOnTapped = true;
        this.SetDynamicResource(BackgroundColorProperty, "PageColor");

        try
        {
            _StatusBarBehavior.StatusBarColor = Application.Current.Resources["Primary"] as Color;
            Behaviors.Add(_StatusBarBehavior);
        } catch { }
    }

    protected override void OnDisappearing()
    {
        Behaviors.Remove(_StatusBarBehavior);
        base.OnDisappearing();
    }
}
