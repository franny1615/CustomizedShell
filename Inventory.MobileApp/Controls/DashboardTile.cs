using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;

namespace Inventory.MobileApp.Controls;

public class DashboardTile : Border
{
    public event EventHandler? Clicked;

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

    public static readonly BindableProperty CountProperty = BindableProperty.Create(
        nameof(Count),
        typeof(int),
        typeof(DashboardTile),
        -1
    );

    public int Count
    {
        get => (int)GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    private readonly VerticalStackLayout _ContentLayout = new() { Spacing = 8 };
    private readonly Label _Title = new Label { MaxLines = 2, HorizontalTextAlignment = TextAlignment.Center }.FontSize(16).Bold();
    private readonly Label _Count = new Label().FontSize(32).Bold().Center();
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
        StrokeShape = new RoundRectangle() { CornerRadius = 12 };

        _ContentLayout.Children.Add(_Count);
        _ContentLayout.Children.Add(_Title);

        _TouchBehavior.Command = new Command(() => { Clicked?.Invoke(this, new EventArgs()); });
        Behaviors.Add(_TouchBehavior);

        Content = new Grid { Children = { _ContentLayout.CenterVertical() } };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");
        _Count.SetDynamicResource(Label.TextColorProperty, "TextColor");
        _Title.SetDynamicResource(Label.TextColorProperty, "TextColor");
        Title = "";
        Count = 0;
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TitleProperty.PropertyName)
        {
            _Title.Text = Title;
        }
        else if (propertyName == CountProperty.PropertyName)
        {
            _Count.Text = $"{Count}";
        }
    }
}
