using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Maui.Inventory;

public class MainPage : ContentPage 
{
    public MainPage()
    {
        Content = new BlazorWebView
        {
            HostPage = "wwwroot/index.html",
            RootComponents = 
            {
                new RootComponent
                {
                    Selector = "#app",
                    ComponentType = typeof(Routes)
                }
            }
        };
    }
}