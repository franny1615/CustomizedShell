using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory;

public class App : Application
{
    #region Constructor
    public App(IDAL<ApiUrl> apiDAL)
    {
        CheckAPIURL(apiDAL);
        
        Resources.MergedDictionaries.Add(new Resources.Styles.Colors());
        Resources.MergedDictionaries.Add(new Resources.Styles.Styles());

        MainPage = new MainPage();
    }
    #endregion

    #region Overrides
    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Resumed += AppResumed;
        window.Stopped += AppStopped;
        window.Created += AppCreated;

        return window;
    }

    protected override void OnStart()
    {
        base.OnStart();
        string keyString = $"{Constants.iOSAppCenterSecret}{Constants.AndroidAppCenterSecret}";
        AppCenter.Start(keyString, typeof(Analytics), typeof(Crashes));
    }

    private void AppResumed(object sender, EventArgs e) { }
    private void AppCreated(object sender, EventArgs e) { }
    private void AppStopped(object sender, EventArgs e) { }

    public async void CheckAPIURL(IDAL<ApiUrl> apiDAL)
    {
        List<ApiUrl> apis = await apiDAL.GetAll();
        if (apis == null || apis.Count == 0)
        {
            ApiUrl prod = new ApiUrl
            {
                URL = "https://mauiinventoryapi20231216094131.azurewebsites.net/"
            };
            await apiDAL.Save(prod);
        }
    }
    #endregion
}
