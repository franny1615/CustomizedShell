using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public class LocationCardView : Border
{
    public event EventHandler? Clicked;

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(StatusCardView), null);
    public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

    public static readonly BindableProperty BarcodeProperty = BindableProperty.Create(nameof(Barcode), typeof(string), typeof(StatusCardView), null);
    public string Barcode { get => (string)GetValue(BarcodeProperty); set => SetValue(BarcodeProperty, value); }

    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(24, 90, 16),
        ColumnDefinitions = Columns.Define(Star, 24),
        RowSpacing = 0,
        ColumnSpacing = 8
    };
    private readonly Label _Description = new Label().Font(size: 16).Bold();
    private readonly Label _BarcodeStr = new Label().Font(size: 12).Bold().CenterHorizontal().Top();
    private readonly SKCanvasView _Barcode = new SKCanvasView();
    private readonly Image _Kebab = new Image().ApplyMaterialIcon(MaterialIcon.More_vert, 24, Color.FromArgb("#383838"));
    private readonly TouchBehavior _TouchBehavior = new TouchBehavior()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.8,
        PressedScale = 0.95
    };

    public LocationCardView()
    {
        Margin = 0;
        Padding = 8;
        StrokeThickness = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");

        _Kebab.Behaviors([_TouchBehavior]);
        _ContentLayout.Add(_Description.Row(0).Column(0));
        _ContentLayout.Add(_Barcode.Row(1).Column(0));
        _ContentLayout.Add(_Kebab.Row(0).RowSpan(3).Column(1).Center());
        _ContentLayout.Add(_BarcodeStr.Row(2).Column(0));

        Content = _ContentLayout;

        _TouchBehavior.Command = new Command(() => Clicked?.Invoke(this, EventArgs.Empty));

        Loaded += HasLoaded;
        Unloaded += HasUnloaded;

        CheckUI();
    }
    ~LocationCardView()
    {
        WeakReferenceMessenger.Default.Unregister<InternalMsg>(this);
        _Barcode.PaintSurface -= PaintBarcode;
    }
    private void HasUnloaded(object? sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Unregister<InternalMsg>(this);
        _Barcode.PaintSurface -= PaintBarcode;
    }
    private void HasLoaded(object? sender, EventArgs e)
    {
        _Barcode.PaintSurface += PaintBarcode;
        WeakReferenceMessenger.Default.Register<InternalMsg>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() => DealWithMessage(msg));
        });
    }

    private void DealWithMessage(InternalMsg msg)
    {
        switch (msg.Value)
        {
            case InternalMessage.ThemeChanged:
                CheckUI();
                break;
        }
    }

    private void CheckUI()
    {
        if (SessionService.CurrentTheme == "dark")
            _Kebab.ApplyMaterialIcon(MaterialIcon.More_vert, 24, Colors.White);
        else
            _Kebab.ApplyMaterialIcon(MaterialIcon.More_vert, 24, Color.FromArgb("#383838"));
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == DescriptionProperty.PropertyName)
        {
            _Description.Text = Description;
        }
        else if (propertyName == BarcodeProperty.PropertyName)
        {
            _BarcodeStr.Text = Barcode;
            _Barcode.InvalidateSurface();
        }
    }

    private void PaintBarcode(object? sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        BarcodeService.DrawCode128Barcode(
            Barcode, 
            e.Surface.Canvas,
            e.Info);
    }
}
