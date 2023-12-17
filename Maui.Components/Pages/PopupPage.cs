﻿using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Controls.Shapes;
using Application = Microsoft.Maui.Controls.Application;

namespace Maui.Components.Pages;

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
        Padding = 16
    };
    #endregion

    #region Constructor
    public PopupPage(ILanguageService languageService) : base(languageService)
    {
        Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false);
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

        this.BackgroundColor = Colors.Transparent;

        On<iOS>().SetUseSafeArea(false);
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FullScreen);

        _ContentContainer.SetAppThemeColor(
            Border.BackgroundColorProperty, 
            Colors.White,
            Color.FromRgb(28,28,30));

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

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
    private void DisplayInfoChange(object sender, DisplayInfoChangedEventArgs e)
    {
        ApplyDimensions(e.DisplayInfo);
    }

    private void ApplyDimensions(DisplayInfo info)
    {
        _OuterLayout.HeightRequest = info.Height / info.Density;
        switch(PopupStyle)
        {
            case PopupStyle.Center:
                _ContentContainer.WidthRequest = info.Width / info.Density * 0.75;
                break;
            case PopupStyle.BottomSheet:
            case PopupStyle.Unknown:
                _ContentContainer.WidthRequest = info.Width / info.Density;
                _ContentContainer.MaximumHeightRequest = info.Height / info.Density * 0.75;
                break;
        }
    }
    #endregion
}