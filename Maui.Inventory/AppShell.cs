﻿using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;
using Maui.Inventory.Pages;
using Maui.Inventory.Services;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;

namespace Maui.Inventory;

public class AppShell : Shell
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly IDAL<User> _UserDAL;
    private readonly FloatingActionButton _LogoutButton = new()
    {
        ImageSource = "logout.png",
        Text = "Logout",
        FABStyle = FloatingActionButtonStyle.Extended,
        HorizontalOptions = LayoutOptions.Start,
        VerticalOptions = LayoutOptions.Center
    };
    private Grid _BrandContainer = new()
    {
        HeightRequest = 150,
        MinimumHeightRequest = DeviceInfo.Current.Platform == DevicePlatform.iOS ? 206 : 150,
        BackgroundColor = Application.Current.Resources["Primary"] as Color,
        Margin = 0,
        RowDefinitions = GridRowsColumns.Rows.Define(100, 30),
        RowSpacing = 4,
        Children = 
        {
            new Image
            {
                HeightRequest = 100,
                WidthRequest = 100,
                Source = "app_ic.png"
            }.Row(0).Bottom(),
        }
    };
    #endregion

    #region Constructor
    public AppShell(
        ILanguageService languageService,
        IDAL<User> userDAL)
    {
        _LanguageService = languageService;
        _UserDAL = userDAL;

        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Flyout);
        Shell.SetTabBarIsVisible(this, true);

        _BrandContainer.Children.Add(new Label
        {
            TextColor = Colors.White,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Text = _LanguageService.StringForKey("WelcomeTo")
        }.Row(1).Top());

        FlyoutHeaderTemplate = new DataTemplate(() => _BrandContainer);
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
                    Title = _LanguageService.StringForKey("Home"), 
                    Icon = "home.png",
                    FlyoutIcon = "home_primary.png",
                    ContentTemplate = new DataTemplate(typeof(MainPage)),
                    Route = nameof(MainPage) 
                },
                new ShellContent
                {
                    Title = _LanguageService.StringForKey("Inventory"),
                    Icon = "inventory.png",
                    FlyoutIcon = "inventory_primary.png",
                    ContentTemplate = new DataTemplate(typeof(InventoryPage)),
                    Route = nameof(InventoryPage)
                },
                new ShellContent
                {
                    Title = _LanguageService.StringForKey("Data"),
                    Icon = "article.png",
                    FlyoutIcon = "article_primary.png",
                    ContentTemplate = new DataTemplate(typeof(DataPage)),
                    Route = nameof(DataPage)
                },
                new ShellContent
                {
                    Title = _LanguageService.StringForKey("Profile"),
                    Icon = "person.png",
                    FlyoutIcon = "person_primary.png",
                    ContentTemplate = new DataTemplate(typeof(ProfilePage)),
                    Route = nameof(ProfilePage)
                },
            }
        });

        Loaded += ShellLoaded;
        Unloaded += ShellUnloaded;

        // these two are here in case we want to display them in Shell.
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

        // actual shell routes
        Routing.RegisterRoute(nameof(StatusesPage), typeof(StatusesPage));
        Routing.RegisterRoute(nameof(CategoriesPage), typeof(CategoriesPage));
        Routing.RegisterRoute(nameof(BarcodesPage), typeof(BarcodesPage));
    }
    #endregion

    #region Helpers
    private void ShellLoaded(object sender, EventArgs e)
    {
        Application.Current.RequestedThemeChanged += ThemeChanged;
        _LogoutButton.Clicked += Logout;
    }

    private void ShellUnloaded(object sender, EventArgs e)
    {
        Application.Current.RequestedThemeChanged -= ThemeChanged;
        _LogoutButton.Clicked -= Logout;
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

    private async void Logout(object sender, EventArgs e)
    {
        // find user that is logged in
        var users = await _UserDAL.GetAll();
        foreach(var user in users)
        {
            if (user.IsLoggedIn)
            {
                user.IsLoggedIn = false;
                await _UserDAL.Save(user);
                break;
            }
        }

        WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-out"));
    }
    #endregion
}