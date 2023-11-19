using CommunityToolkit.Maui.Behaviors;
using CustomizedShell.Services;

namespace CustomizedShell.Pages;

public class BasePage : ContentPage
{
    internal LanguageService Lang => LanguageService.Instance;
    private readonly StatusBarBehavior _StatusBarBehavior = new();

    protected override void OnAppearing()
    {
        base.OnAppearing();
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

    public void OverrideBackButtonText()
    {
        // android default back button is good enough 
        #if IOS
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            TextOverride = Lang["GoBack"]
        });
        #endif
    }
}
