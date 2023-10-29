using CustomizedShell.Pages;
using Maui.Components.Controls;

namespace CustomizedShell;

public class AppShell : Shell
{
    private readonly FloatingActionButton _LogoutButton = new()
    {
        ImageSource = "logout.png",
        Text = "Logout",
        FABStyle = FloatingActionButtonStyle.Extended,
        HorizontalOptions = LayoutOptions.Start,
        VerticalOptions = LayoutOptions.Center
    };

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

        FlyoutFooter = new Grid
        {
            HeightRequest = 100,
            Padding = 16,
            Children = 
            {
                _LogoutButton
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

        Loaded += ShellLoaded;
        Unloaded += ShellUnloaded;
    }

    private void ShellLoaded(object sender, EventArgs e)
    {
        Application.Current.RequestedThemeChanged += ThemeChanged;
    }

    private void ShellUnloaded(object sender, EventArgs e)
    {
        Application.Current.RequestedThemeChanged -= ThemeChanged;
    }

    private void ThemeChanged(object sender, AppThemeChangedEventArgs e)
    {
        if (e.RequestedTheme == AppTheme.Dark)
        {
            _LogoutButton.TextColor = Colors.White;
        }
        else
        {
            _LogoutButton.TextColor = Colors.Black;
        }
    }
}
