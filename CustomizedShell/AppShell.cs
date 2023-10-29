using CustomizedShell.Pages;

namespace CustomizedShell;

public class AppShell : Shell
{
    public AppShell()
    {
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
        Shell.SetTabBarIsVisible(this, true);

        FlyoutHeader = new Grid 
        {
            HeightRequest = 110,
            Margin = 0,
            BackgroundColor = Color.FromArgb("#3EB489"),
            Children = 
            {
                new Image
                {
                    Source = "home.png",
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            }
        };

        var button = new Button
        {
            Padding = 16,
            BackgroundColor = Colors.Transparent,
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
            ImageSource = "logout.png",
            Text = "Logout",
            TextColor = Color.FromArgb("#3EB489"),
            HorizontalOptions = LayoutOptions.Start
        };

        FlyoutFooter = new Grid
        {
            Children = 
            {
                button
            }
        };
        
        Items.Add(new FlyoutItem
        {
            FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
            Items =
            {
                new ShellContent 
                { 
                    Title = "Home", 
                    Icon = "home.png",
                    FlyoutIcon = "home_green.png",
                    ContentTemplate = new DataTemplate(typeof(MainPage)),
                    Route = nameof(MainPage) 
                },
                new ShellContent 
                { 
                    Title = "Search", 
                    Icon = "search.png",
                    FlyoutIcon = "search_green.png",
                    ContentTemplate = new DataTemplate(typeof(SearchPage)),
                    Route = nameof(SearchPage) 
                },
                new ShellContent 
                { 
                    Title = "Settings", 
                    Icon = "settings.png",
                    FlyoutIcon = "settings_green.png",
                    ContentTemplate = new DataTemplate(typeof(SettingsPage)),
                    Route = nameof(SettingsPage) 
                }
            }
        });
    }
}
