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
    public event EventHandler? Selected;

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(LocationCardView), null);
    public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

    public static readonly BindableProperty BarcodeProperty = BindableProperty.Create(nameof(Barcode), typeof(string), typeof(LocationCardView), null);
    public string Barcode { get => (string)GetValue(BarcodeProperty); set => SetValue(BarcodeProperty, value); }

    public static readonly BindableProperty IsSelectableProperty = BindableProperty.Create(nameof(IsSelectable), typeof(bool), typeof(LocationCardView), false);
    public bool IsSelectable { get => (bool)GetValue(IsSelectableProperty); set => SetValue(IsSelectableProperty, value); }

    public byte[] CurrentBarcode = [];

    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(24, 90),
        ColumnDefinitions = Columns.Define(Star, 24),
        RowSpacing = 0,
        ColumnSpacing = 8
    };
    private readonly Label _Description = new Label().Font(size: 16).Bold();
    private readonly SKCanvasView _Barcode = new SKCanvasView();
    private readonly Image _Kebab = new Image().ApplyMaterialIcon(MaterialIcon.More_vert, 24, Color.FromArgb("#383838"));
    private readonly TouchBehavior _TouchBehavior = new TouchBehavior()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.8,
        PressedScale = 0.95
    };
    private Action<byte[]>? GetImageCompletion;

    private readonly TapGestureRecognizer _SelectGesture = new TapGestureRecognizer { NumberOfTapsRequired = 1 };

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
        _ContentLayout.Add(_Kebab.Row(0).RowSpan(2).Column(1).Center());

        Content = _ContentLayout;

        _TouchBehavior.Command = new Command(() => Clicked?.Invoke(this, EventArgs.Empty));
        _SelectGesture.Tapped += async (s, e) => {
            await this.ScaleTo(0.95, 70);
            await this.ScaleTo(1.0, 70);

            Selected?.Invoke(this, EventArgs.Empty);
        };

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
            _Barcode.InvalidateSurface();
        }
        else if (propertyName == IsSelectableProperty.PropertyName)
        {
            _ContentLayout.Remove(_Kebab);
            if (IsSelectable)
            {
                GestureRecognizers.Add(_SelectGesture);
            }
            else
            {
                GestureRecognizers.Remove(_SelectGesture);
                _ContentLayout.Add(_Kebab);
            }
        }
    }

    private void PaintBarcode(object? sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        if (_Barcode == null || string.IsNullOrEmpty(Barcode))
            return;
        
        BarcodeService.DrawCode128Barcode(
            Barcode,
            e.Surface.Canvas,
            e.Info);

        var snap = e.Surface.Snapshot();
        var image = SKBitmap.Decode(snap.Encode());
        CurrentBarcode = image.Encode(SKEncodedImageFormat.Png, 100).ToArray();
    }
}
