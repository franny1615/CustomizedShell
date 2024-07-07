using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System.Globalization;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public class InventoryCardView : Border
{
    public event EventHandler? EditDescription;
    public event EventHandler? EditStatus;
    public event EventHandler? EditQuantity;
    public event EventHandler? EditQuantityType;
    public event EventHandler? EditLocation;
    public event EventHandler? KebabMenu;

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(InventoryCardView), null);
    public string Description { get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }

    public static readonly BindableProperty StatusProperty = BindableProperty.Create(nameof(Status), typeof(string), typeof(InventoryCardView), null);
    public string Status { get => (string)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

    public static readonly BindableProperty QuantityProperty = BindableProperty.Create(nameof(Quantity), typeof(int), typeof(InventoryCardView), -1);
    public int Quantity { get => (int)GetValue(QuantityProperty); set => SetValue(QuantityProperty, value); }

    public static readonly BindableProperty QuantityTypeProperty = BindableProperty.Create(nameof(QuantityType), typeof(string), typeof(InventoryCardView), null);
    public string QuantityType { get => (string)GetValue(QuantityTypeProperty); set => SetValue(QuantityTypeProperty, value); }

    public static readonly BindableProperty BarcodeProperty = BindableProperty.Create(nameof(Barcode), typeof(string), typeof(InventoryCardView), null);
    public string Barcode { get => (string)GetValue(BarcodeProperty); set => SetValue(BarcodeProperty, value); }

    public static readonly BindableProperty LocationProperty = BindableProperty.Create(nameof(Location), typeof(string), typeof(InventoryCardView), null);
    public string Location { get => (string)GetValue(LocationProperty); set => SetValue(LocationProperty, value); }

    public static readonly BindableProperty LastEditedOnProperty = BindableProperty.Create(nameof(LastEditedOn), typeof(DateTime), typeof(InventoryCardView), new DateTime(1970, 1, 1));
    public DateTime LastEditedOn { get => (DateTime)GetValue(LastEditedOnProperty); set => SetValue(LastEditedOnProperty, value); }

    public static readonly BindableProperty CreatedOnProperty = BindableProperty.Create(nameof(CreatedOn), typeof(DateTime), typeof(InventoryCardView), new DateTime(1970, 1, 1));
    public DateTime CreatedOn { get => (DateTime)GetValue(CreatedOnProperty); set => SetValue(CreatedOnProperty, value); }

    public static readonly BindableProperty HideKebabProperty = BindableProperty.Create(nameof(HideKebab), typeof(bool), typeof(InventoryCardView), false);
    public bool HideKebab { get => (bool)GetValue(HideKebabProperty); set => SetValue(HideKebabProperty, value); }

    private readonly IconLabel _Description = new()
    {
        Header = LanguageService.Instance["Description"],
        Icon = MaterialIcon.Edit,
    };
    private readonly IconLabel _Quantity = new()
    {
        Header = LanguageService.Instance["Quantity"],
        Icon = MaterialIcon.Edit
    };
    private readonly IconLabel _QtyType = new()
    {
        Header = LanguageService.Instance["Quantity Type"],
        Icon = MaterialIcon.Edit
    };
    private readonly IconLabel _Status = new()
    {
        Header = LanguageService.Instance["Status"],
        Icon = MaterialIcon.Edit
    };
    private readonly IconLabel _Location = new()
    {
        Header = LanguageService.Instance["Location"],
        Icon = MaterialIcon.Edit
    };
    private readonly IconLabel _LastEditedOn = new()
    {
        Header = LanguageService.Instance["Last Edited"],
    };
    private readonly IconLabel _CreatedOn = new()
    {
        Header = LanguageService.Instance["Created"]
    };
    private readonly Image _Kebab = new Image();
    private readonly SKCanvasView _Barcode = new() { HeightRequest = 120 };
    public byte[] CurrentBarcode = [];

    public InventoryCardView()
    {
        Margin = 0;
        Padding = 8;
        StrokeThickness = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");

        _Description.TapGesture(() => EditDescription?.Invoke(this, EventArgs.Empty));
        _Quantity.TapGesture(() => EditQuantity?.Invoke(this, EventArgs.Empty));
        _QtyType.TapGesture(() => EditQuantityType?.Invoke(this, EventArgs.Empty));
        _Status.TapGesture(() => EditStatus?.Invoke(this, EventArgs.Empty));
        _Location.TapGesture(() => EditLocation?.Invoke(this, EventArgs.Empty));
        _Kebab.TapGesture(() => KebabMenu?.Invoke(this, EventArgs.Empty));

        var flex = new FlexLayout
        {
            ZIndex = 0,
            Direction = FlexDirection.Row,
            JustifyContent = FlexJustify.Center,
            Wrap = FlexWrap.Wrap,
            Children =
            {
                _Description,
                _Quantity,
                _QtyType,
                _Status,
                _Location,
                _LastEditedOn,
                _CreatedOn,
                _Barcode
            }
        };

        Content = new VerticalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new Grid
                {
                    ColumnSpacing = 12,
                    ColumnDefinitions = Columns.Define(Star, 32),
                    Children =
                    {
                        _Description,
                        _Kebab.Column(1).Center()
                    }
                },
                new Grid
                {
                    ColumnSpacing = 12,
                    ColumnDefinitions = Columns.Define(Star, Star),
                    Children =
                    {
                        _Quantity,
                        _QtyType.Column(1)
                    }
                },
                _Status,
                _Location,
                new Grid
                {
                    ColumnSpacing = 12,
                    ColumnDefinitions = Columns.Define(Star, Star),
                    Children =
                    {
                        _LastEditedOn,
                        _CreatedOn.Column(1)
                    }
                },
                _Barcode
            }
        };

        _Barcode.PaintSurface += PaintBarcode;

        KebabMenuColor();
        WeakReferenceMessenger.Default.Register<InternalMsg>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (msg.Value)
                {
                    case InternalMessage.ThemeChanged:
                        KebabMenuColor();
                        break;
                }
            });
        });
    }

    private void KebabMenuColor()
    {
        Color color = SessionService.CurrentTheme == "dark" ? Color.FromArgb("#c7c7cc") : Color.FromArgb("#646464");
        _Kebab.ApplyMaterialIcon(MaterialIcon.More_vert, 32, color).ZIndex(1).Top().End();
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

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == DescriptionProperty.PropertyName)
        {
            _Description.Text = Description;
        }
        else if (propertyName == QuantityProperty.PropertyName)
        {
            _Quantity.Text = Quantity.ToString();
        }
        else if (propertyName == QuantityTypeProperty.PropertyName)
        {
            _QtyType.Text = QuantityType;
        }
        else if (propertyName == StatusProperty.PropertyName)
        {
            _Status.Text = Status;
        }
        else if (propertyName == LocationProperty.PropertyName)
        {
            _Location.Text = Location;
        }
        else if (propertyName == LastEditedOnProperty.PropertyName)
        {
            _LastEditedOn.Text = LastEditedOn.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
        }
        else if (propertyName == CreatedOnProperty.PropertyName)
        {
            _CreatedOn.Text = CreatedOn.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
        }
        else if (propertyName == BarcodeProperty.PropertyName)
        {
            _Barcode.InvalidateSurface();
        }
        else if (propertyName == HideKebabProperty.PropertyName)
        {
            _Kebab.IsVisible = !HideKebab;
        }
    }
}

public class IconLabel : Grid
{
    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(nameof(Header), typeof(string), typeof(IconLabel), null);
    public string Header { get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(IconLabel), null);
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(IconLabel), null);
    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }

    private readonly Label _Header = new Label().FontSize(10).TextColor(Color.FromArgb("#c7c7cc")).Bold().Start();
    private readonly Label _Text = new Label { MaxLines = 3, LineBreakMode = LineBreakMode.WordWrap }.FontSize(16).Bold();
    private readonly Image _Icon = new();

    public IconLabel()
    {
        Margin = 0;
        Padding = 0;
        RowSpacing = 0;
        ColumnSpacing = 8;
        MinimumHeightRequest = 48;

        ColumnDefinitions = Columns.Define(Star, 16);
        RowDefinitions = Rows.Define(Auto, Star);

        WeakReferenceMessenger.Default.Register<InternalMsg>(this, (_, msg) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (msg.Value)
                {
                    case InternalMessage.ThemeChanged:
                        Color color = SessionService.CurrentTheme == "dark" ? Color.FromArgb("#c7c7cc") : Color.FromArgb("#646464");
                        UpdateIcon(color);
                        UpdateHeader(color);
                        break;
                }
            });
        });
    }

    private void UpdateIcon(Color color)
    {
        if (_Icon != null && !string.IsNullOrEmpty(Icon))
        {
            _Icon.ApplyMaterialIcon(Icon, 14, color);
        }
    }

    private void UpdateHeader(Color color)
    {
        if (_Header != null && !string.IsNullOrEmpty(Header))
        {
            _Header.TextColor = color;
            _Header.Text = Header;
        }
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        Color color = SessionService.CurrentTheme == "dark" ? Color.FromArgb("#c7c7cc") : Color.FromArgb("#646464");
        if (propertyName == TextProperty.PropertyName)
        {
            _Text.Text = Text;
            LayoutItems();
        }
        else if (propertyName == IconProperty.PropertyName)
        {
            UpdateIcon(color);
            LayoutItems();
        }
        else if (propertyName == HeaderProperty.PropertyName)
        {
            UpdateHeader(color);
            LayoutItems();
        }
    }

    private void LayoutItems()
    {
        Clear();
        if (!string.IsNullOrEmpty(Header))
        {
            Add(_Header.Row(0).Column(0).ColumnSpan(2));
            LayoutTextItems();
        }
        else
        {
            LayoutTextItems();
        }       
    }

    private void LayoutTextItems()
    {
        if (string.IsNullOrEmpty(Icon))
        {
            Add(_Text.Row(1).Column(0).ColumnSpan(2));
        }
        else
        {
            Add(_Text.Row(1).Column(0).ColumnSpan(1));
            Add(_Icon.Row(1).Column(1));
        }
    }
}
