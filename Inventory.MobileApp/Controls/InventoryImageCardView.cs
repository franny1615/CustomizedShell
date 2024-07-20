using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public class InventoryImageCardView : Border 
{
    public event EventHandler? DownloadImage;
    public event EventHandler? DeleteImage;
    public event EventHandler? EditImage;

    public static readonly BindableProperty ImageBase64Property = BindableProperty.Create(nameof(ImageBase64), typeof(string), typeof(InventoryImageCardView));
    public string ImageBase64 { get => (string)GetValue(ImageBase64Property); set => SetValue(ImageBase64Property, value); }

    private readonly Image _InvImage = new Image().Width(120).Height(120).Center();
    private readonly Image _DownloadIcon = new Image().ApplyMaterialIcon(MaterialIcon.Download, 24, Color.FromArgb("#3C6997")).Center();
    private readonly Image _DeleteIcon = new Image().ApplyMaterialIcon(MaterialIcon.Delete, 24, Colors.Red).Center();
    private readonly Image _EditIcon = new Image().ApplyMaterialIcon(MaterialIcon.Edit, 24, Color.FromArgb("#3C6997")).Center();

    private readonly VerticalStackLayout _ContentLayout = new VerticalStackLayout
    {
        Spacing = 8,
    };
    private readonly Grid _OptionsLayout = new Grid
    {
        ColumnDefinitions = Columns.Define(Star, Star, Star),
        ColumnSpacing = 8
    };

    public InventoryImageCardView()
    {
        Margin = 0;
        Padding = 8;
        StrokeThickness = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        SetDynamicResource(Border.BackgroundProperty, "DashTileColor");

        _OptionsLayout.Add(_EditIcon.Column(0));
        _OptionsLayout.Add(_DownloadIcon.Column(1));
        _OptionsLayout.Add(_DeleteIcon.Column(2));

        _ContentLayout.Add(_InvImage);
        _ContentLayout.Add(_OptionsLayout);

        Content = _ContentLayout;

        _EditIcon.TapGesture(() => EditImage?.Invoke(this, EventArgs.Empty));
        _DownloadIcon.TapGesture(() => DownloadImage?.Invoke(this, EventArgs.Empty));
        _DeleteIcon.TapGesture(() => DeleteImage?.Invoke(this, EventArgs.Empty));
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == ImageBase64Property.PropertyName && !string.IsNullOrEmpty(ImageBase64))
        {
            try
            {
                byte[] img = Convert.FromBase64String(ImageBase64);
                MemoryStream stream = new MemoryStream(img);
                _InvImage.Source = ImageSource.FromStream(() => stream);
            }
            catch { }
        }
    }
}
