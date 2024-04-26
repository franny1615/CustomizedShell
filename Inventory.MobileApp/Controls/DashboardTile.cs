using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
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

    public static BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(DashboardTile),
        Colors.Black
    );

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(string),
        typeof(DashboardTile),
        null
    );

    public string Icon 
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private readonly Grid _ContentLayout = new Grid();
    private readonly Image _Icon = new Image().Start().CenterVertical();
    private readonly Label _Title = new Label().FontSize(21).Bold().Start().CenterVertical();
    private readonly Label _Count = new Label().FontSize(40).Top().Bold().End().Padding(new Thickness(0, 0, 8, 0));
    private readonly TouchBehavior _TouchBehavior = new TouchBehavior()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.6,
        PressedScale = 0.8
    };

    public DashboardTile()
    {
        this.Padding(new Thickness(8, 12, 8, 12));

        FlexLayout.SetGrow(this, 0);
        StrokeShape = new RoundRectangle() { CornerRadius = 12 };
        Margin = 4;

        _ContentLayout.Children.Add(_Count);
        _ContentLayout.Children.Add(new VerticalStackLayout
        {
            Spacing = 4,
            Children = { _Icon, _Title }
        }.Bottom());

        _TouchBehavior.Command = new Command(() => 
        {
            Clicked?.Invoke(this, new EventArgs());    
        });

        this.Behaviors.Add(_TouchBehavior);

        Content = _ContentLayout;

        BackgroundColor = Application.Current!.Resources["Primary"] as Color;
        TextColor = Colors.White;
        Title = "";
        Count = 0;
        Icon = MaterialIcon.Question_mark;

        Loaded += HasLoaded;
        Unloaded += HasUnloaded;
    }

    private void HasLoaded(object? sender, EventArgs e)
    {
        ApplySize();
        DeviceDisplay.Current.MainDisplayInfoChanged += DisplayInfoChanged;
    }

    private void HasUnloaded(object? sender, EventArgs e)
    {
        DeviceDisplay.Current.MainDisplayInfoChanged -= DisplayInfoChanged;
    }

    private void DisplayInfoChanged(object? sender, EventArgs e)
    {
        ApplySize();
    }

    private void ApplySize()
    {
        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        var width = displayInfo.Width / displayInfo.Density;
        
        WidthRequest = width * 0.45;
        MaximumWidthRequest = width * 0.45;
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TextColorProperty.PropertyName)
        {
            _Title.TextColor = TextColor;
            _Count.TextColor = TextColor;
        }
        else if (propertyName == TitleProperty.PropertyName)
        {
            _Title.Text = Title;
        }
        else if (propertyName == CountProperty.PropertyName)
        {
            _Count.Text = $"{Count}";
        }
        else if (propertyName == IconProperty.PropertyName)
        {
            _Icon.ApplyMaterialIcon(Icon, 32, TextColor);
        }
    }
}
