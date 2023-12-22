using CommunityToolkit.Mvvm.Messaging;
using Maui.Inventory.Models;
using Maui.Components;
using Maui.Components.Interfaces;
using Maui.Components.Pages;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.Inventory.Pages;

public class SplashScreen : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<ApiUrl> _ApiDAL;
    #endregion

    #region Constructor
    public SplashScreen(
        ILanguageService languageService, 
        IDAL<User> userDAL,
        IDAL<ApiUrl> apiDAL) : base(languageService)
    {
        _LanguageService = languageService;
        _UserDAL = userDAL;
        _ApiDAL = apiDAL;

        NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

        Content = new Grid
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Border
                {
                    Padding = 0,
                    Margin = 0,
                    BackgroundColor = Application.Current.Resources["Primary"] as Color,
                    Stroke = Colors.Transparent,
                    StrokeShape = new RoundRectangle { CornerRadius = 24 },
                    Content = new Image
                    {
                        WidthRequest = 124,
                        HeightRequest = 124,
                        Source = "app_ic.png"
                    }
                }
            }
        };
    }
    #endregion

    #region Overrides
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        await SetupAPI();

        var users = await _UserDAL.GetAll();

        if (users != null && users.Count > 0)
        {
            var loggedInUser = GetLoggedInUser(users);
            if (loggedInUser != null)
            {
                WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-in"));
            }
            else
            {
                WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-out"));
            }
        }
        else
        {
            WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-out"));
        }
    }

    private User GetLoggedInUser(List<User> users)
    {
        foreach (var user in users)
        {
            if (user.IsLoggedIn)
            {
                return user;
            }
        }

        return null;
    }

    private async Task SetupAPI()
    {
        // check if we have 
        int apiUrlId = Preferences.Get(Constants.ApiUrlId, -1);
        if (apiUrlId != -1)
            return;

        // setup API URLs
        var api = await _ApiDAL.GetAll();
        foreach (var item in api)
        {
            await _ApiDAL.Delete(item);
        }
        ApiUrl dev = new() { URL = "https://localhost:7263" };
        ApiUrl prod = new() { URL = "https://mauiinventoryapi20231216094131.azurewebsites.net" };
        await _ApiDAL.Save(dev);
        await _ApiDAL.Save(prod);
        Preferences.Set(Constants.ApiUrlId, prod.Id);
    }
    #endregion
}
