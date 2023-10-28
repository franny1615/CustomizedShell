using CustomizedShell.Pages;

namespace CustomizedShell;

public class AppShell : Shell
{
    public AppShell()
    {
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
        Shell.SetTabBarIsVisible(this, true);
        
        Items.Add(new ShellContent 
        { 
            Title = "Home", 
            ContentTemplate = new DataTemplate(typeof(MainPage)),
            Route = nameof(MainPage) 
        });
    }
}
