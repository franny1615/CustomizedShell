using System.Runtime.CompilerServices;

namespace Maui.Components.Controls;

public class MaterialImage : Image
{
    public static BindableProperty IconProperty = BindableProperty.Create(
        nameof(IconProperty),
        typeof(string),
        typeof(MaterialImage),
        null
    );

    public string Icon 
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSizeProperty),
        typeof(int),
        typeof(MaterialImage),
        0
    );

    public int IconSize 
    {
        get => (int)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColorProperty),
        typeof(Color),
        typeof(MaterialImage),
        null
    );

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    private FontImageSource _IconSource = new();

    public MaterialImage()
    {
        _IconSource.FontFamily = MaterialIcon.FontName;
        Source = _IconSource;
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == IconColorProperty.PropertyName)
        {
            _IconSource.Color = IconColor;
        }
        else if (propertyName == IconSizeProperty.PropertyName)
        {
            _IconSource.Size = IconSize;
        }
        else if (propertyName == IconProperty.PropertyName)
        {
            _IconSource.Glyph = Icon;
        }
    }
}