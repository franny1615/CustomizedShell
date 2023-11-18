using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using CustomizedShell.Services;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace CustomizedShell.Pages;

public class BasePage : ContentPage
{
    internal LanguageService Lang => LanguageService.Instance;
    private readonly StatusBarBehavior _StatusBarBehavior = new();
    public readonly Grid ContentLayout = new()
    {
        RowDefinitions = Rows.Define(50, Star)
    };
    public readonly ContentView Page = new();

    // TODO: embed the navbar onto this page

    public BasePage()
    {
        Shell.SetNavBarIsVisible(this, false);
        ContentLayout.Children.Add(Page.Column(1));

        Content = ContentLayout;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _StatusBarBehavior.StatusBarColor = Application.Current.Resources["Primary"] as Color;
        Behaviors.Add(_StatusBarBehavior);
    }

    protected override void OnDisappearing()
    {
        Behaviors.Remove(_StatusBarBehavior);
        base.OnDisappearing();
    }

    public void HideNavBar()
    {
        // TODO: hide the navbar
        Page.RowSpan(2);
    }
}
