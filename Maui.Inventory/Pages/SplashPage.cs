using CommunityToolkit.Mvvm.Messaging;
using Maui.Components;
using Maui.Components.Pages;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace Maui.Inventory.Pages;

public class SplashPage : BasePage
{
	#region Private Properties
	private SplashViewModel _ViewModel => (SplashViewModel)BindingContext;
	#endregion

	#region Constructor
	public SplashPage(
		ILanguageService languageService,
		SplashViewModel splashViewModel) : base(languageService)
	{
		BindingContext = splashViewModel;

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
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ViewModel.CheckAPIURL();
        CheckAuth();
    }

    private async void CheckAuth()
    {
        AccessMessage auth = await _ViewModel.IsAccessTokenValid();
        WeakReferenceMessenger.Default.Send(new InternalMessage(auth));
    }
    #endregion
}