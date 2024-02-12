using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class MaterialTile : Border
{
    #region Events
    public event EventHandler Clicked;
    #endregion

    #region Public Properties
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(MaterialTile), null);
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(MaterialTile), null);
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty ForegroundColorProperty = BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(MaterialTile), null);
    public Color ForegroundColor
    {
        get => (Color)GetValue(ForegroundColorProperty);
        set => SetValue(ForegroundColorProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new()
    {
        Padding = 8,
        RowSpacing = 8,
        ColumnSpacing = 8,
        RowDefinitions = Rows.Define(Star, Star),
    };
    private readonly MaterialImage _IconImg = new()
    {
        IconSize = 30
    };
    private readonly Label _TitleLabel = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
    };
    #endregion

    #region Constructor
    public MaterialTile()
    {
        Stroke = Colors.Transparent;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        _ContentLayout.Children.Add(_IconImg.Row(0).Column(0).Center());
        _ContentLayout.Children.Add(_TitleLabel.Row(1).Column(0).Center());

        this.TapGesture(async () =>
        {
            await this.ScaleTo(0.95, 70);
            await this.ScaleTo(1.0, 70);

            Clicked?.Invoke(this, null);
        });

        Content = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TitleProperty.PropertyName)
        {
            _TitleLabel.Text = Title;
        }
        else if (propertyName == IconProperty.PropertyName)
        {
            _IconImg.Icon = Icon;
        }
        else if (propertyName == ForegroundColorProperty.PropertyName) 
        {
            _TitleLabel.TextColor = ForegroundColor;
            _IconImg.IconColor = ForegroundColor;
        }
    }
    #endregion
}
