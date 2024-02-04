using Maui.Inventory.ViewModels;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory;

public class App : Application
{
    #region Private Properties
    private readonly AppViewModel _AppVM;
    #endregion

    #region Constructor
    public App(AppViewModel appVM)
    {
        _AppVM = appVM;
        _AppVM.CheckAPIURL();
        
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
    #endregion
}
