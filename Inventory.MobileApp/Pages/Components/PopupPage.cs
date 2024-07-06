using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;

namespace Inventory.MobileApp.Pages.Components;

public enum PopupStyle
{
    Center,
    BottomSheet,
    Unknown
}

public class PopupPage : BasePage
{
    #region Public Properties
    public static readonly BindableProperty PopupStyleProperty = BindableProperty.Create(
        nameof(PopupStyleProperty),
        typeof(PopupStyle),
        typeof(PopupPage),
        PopupStyle.Unknown
    );

    public PopupStyle PopupStyle
    {
        set => SetValue(PopupStyleProperty, value);
        get => (PopupStyle)GetValue(PopupStyleProperty);
    }

    public static readonly BindableProperty PopupContentProperty = BindableProperty.Create(
        nameof(PopupContentProperty),
        typeof(View),
        typeof(PopupPage),
        null
    );

    public View PopupContent
    {
        get => (View)GetValue(PopupContentProperty);
        set => SetValue(PopupContentProperty, value);
    }
    #endregion

    #region Private Variables
    private readonly Grid _OuterLayout = new()
    {
        IgnoreSafeArea = true,
        BackgroundColor = Colors.Transparent
    };
    private readonly ContentView _ContentLayout = new();
    private readonly Border _ContentContainer = new()
    {
        Stroke = Colors.Transparent,
        StrokeShape = new RoundRectangle { CornerRadius = 16 },
        Padding = 0
    };
    #endregion

    #region Constructor
    public PopupPage()
    {
        Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

        BackgroundColor = Colors.Transparent;

        On<iOS>().SetUseSafeArea(false);
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FullScreen);

        _ContentContainer.SetDynamicResource(BackgroundColorProperty, "PopupColor");

        _ContentContainer.Content = _ContentLayout;
        _OuterLayout.Children.Add(_ContentContainer);
        Content = _OuterLayout;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        DeviceDisplay.Current.MainDisplayInfoChanged += DisplayInfoChange;
        Task.Run(async () =>
        {
            await Task.Delay(250);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _OuterLayout.BackgroundColorTo(Colors.Black.WithAlpha(0.25f), easing: Easing.CubicIn);
            });
        });
    }

    protected override void OnDisappearing()
    {
        DeviceDisplay.Current.MainDisplayInfoChanged -= DisplayInfoChange;
        _OuterLayout.BackgroundColor = Colors.Transparent;
        base.OnDisappearing();
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == PopupStyleProperty.PropertyName)
        {
            switch (PopupStyle)
            {
                case PopupStyle.Center:
                    _ContentContainer.Center();
                    ApplyDimensions(DeviceDisplay.Current.MainDisplayInfo);
                    break;
                case PopupStyle.BottomSheet:
                case PopupStyle.Unknown:
                    _ContentContainer.Bottom();
                    ApplyDimensions(DeviceDisplay.Current.MainDisplayInfo);
                    break;
            }
        }
        else if (propertyName == PopupContentProperty.PropertyName)
        {
            _ContentLayout.Content = PopupContent;
        }
    }
    #endregion

    #region Helpers
    private void DisplayInfoChange(object? sender, DisplayInfoChangedEventArgs e)
    {
        ApplyDimensions(e.DisplayInfo);
    }

    private void ApplyDimensions(DisplayInfo info)
    {
        _OuterLayout.HeightRequest = info.Height / info.Density;
        switch (PopupStyle)
        {
            case PopupStyle.Center:
                _ContentContainer.WidthRequest = info.Width / info.Density * 0.85;
                #if ANDROID
                _ContentContainer.MinimumHeightRequest = info.Height / info.Density * 0.35;
                _ContentContainer.MaximumHeightRequest = info.Height / info.Density * 0.85;
                #endif
                #if IOS
                _ContentContainer.HeightRequest = info.Height / info.Density * 0.85;
                #endif
                break;
            case PopupStyle.BottomSheet:
            case PopupStyle.Unknown:
                _ContentContainer.WidthRequest = info.Width / info.Density;
                _ContentContainer.HeightRequest = info.Height / info.Density * 0.75;
                break;
        }
    }
    #endregion
}
