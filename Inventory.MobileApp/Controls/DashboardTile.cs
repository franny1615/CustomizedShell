using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace Inventory.MobileApp.Controls;

public class DashboardTile : Border
{
    public event EventHandler? Clicked;

    public static readonly BindableProperty TypeProperty = BindableProperty.Create(
        nameof(Type),
        typeof(DashboardItemType),
        typeof(DashboardTile),
        DashboardItemType.Unknown);

    public DashboardItemType Type
    {
        get => (DashboardItemType)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(DashboardTile),
        null);
    
    public string Title 
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private readonly VerticalStackLayout _ContentLayout = new() { Spacing = 8 };
    private readonly Image _Icon = new Image();
    private readonly Label _Title = new Label { MaxLines = 2, HorizontalTextAlignment = TextAlignment.Center }.FontSize(16).Bold();
    private readonly TouchBehavior _TouchBehavior = new TouchBehavior()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.8,
        PressedScale = 0.95
    };

    public DashboardTile()
    {
        FlexLayout.SetBasis(this, new Microsoft.Maui.Layouts.FlexBasis(0.5f, true));
        Padding = 8;
        Margin = new Thickness(4, 4, 4, 32);

        Shadow = new Shadow
        {
            Brush = Color.FromArgb("#646464"),
            Offset = new Point(1, 1),
            Radius = 40,
            Opacity = 0.25f
        };
        StrokeShape = new RoundRectangle() { CornerRadius = 5 };

        _ContentLayout.Children.Add(_Icon.CenterHorizontal());
        _ContentLayout.Children.Add(_Title);

        _TouchBehavior.Command = new Command(() => { Clicked?.Invoke(this, new EventArgs()); });
        Behaviors.Add(_TouchBehavior);

        Content = new Grid { Children = { _ContentLayout.CenterVertical() } };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");
        _Title.SetDynamicResource(Label.TextColorProperty, "TextColor");
        Title = "";

        WeakReferenceMessenger.Default.Register<InternalMsg>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (msg.Value)
                {
                    case InternalMessage.ThemeChanged:
                        ApplyIcon();
                        break;
                }
            });
        });
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TitleProperty.PropertyName)
        {
            _Title.Text = Title;
        }
        else if (propertyName == TypeProperty.PropertyName)
        {
            ApplyIcon();
        }
    }

    private void ApplyIcon()
    {
        Color color = SessionService.CurrentTheme == "dark" ? Color.FromArgb("#c7c7cc") : Color.FromArgb("#646464");
        var iconSize = 40;
        switch (Type)
        {
            case DashboardItemType.Inventory:
                _Icon.ApplyMaterialIcon(MaterialIcon.Inventory_2, iconSize, color);
                break;
            case DashboardItemType.Employees:
                _Icon.ApplyMaterialIcon(MaterialIcon.Groups, iconSize, color);
                break;
            case DashboardItemType.Statuses:
                _Icon.ApplyMaterialIcon(MaterialIcon.Beenhere, iconSize, color);
                break;
            case DashboardItemType.Locations:
                _Icon.ApplyMaterialIcon(MaterialIcon.Location_on, iconSize, color);
                break;
            case DashboardItemType.QuantityTypes:
                _Icon.ApplyMaterialIcon(MaterialIcon.Tag, iconSize, color);
                break;
            case DashboardItemType.Profile:
                _Icon.ApplyMaterialIcon(MaterialIcon.Person, iconSize, color);
                break;
            case DashboardItemType.Unknown:
                break;
        }
    }
}
