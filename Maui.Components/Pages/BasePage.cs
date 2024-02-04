using CommunityToolkit.Maui.Behaviors;

namespace Maui.Components.Pages;

public class BasePage(string backButtonText) : ContentPage
{
    private readonly StatusBarBehavior _StatusBarBehavior = new();

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

    public void OverrideBackButtonText()
    {
        // android default back button is good enough 
        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            Shell.SetBackButtonBehavior(this, new BackButtonBehavior
            {
                TextOverride = backButtonText
            });
        }
    }
}
