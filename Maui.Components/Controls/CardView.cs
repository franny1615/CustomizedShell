using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class CardView : ContentView
{
    #region Events
    public event EventHandler Clicked;
    #endregion

    #region Public Properties
    public static readonly BindableProperty ImageBackgroundColorProperty = BindableProperty.Create(
        nameof(ImageBackgroundColorProperty),
        typeof(Color),
        typeof(CardView),
        null);

    public Color ImageBackgroundColor
    {
        get => (Color)GetValue(ImageBackgroundColorProperty);
        set => SetValue(ImageBackgroundColorProperty, value);
    }

    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSourceProperty),
        typeof(ImageSource),
        typeof(CardView),
        null
    );

    public ImageSource ImageSource 
    {
        get => (ImageSource) GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(TitleProperty),
        typeof(string),
        typeof(CardView),
        null);

    public string Title
    {
        get => (string) GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(
        nameof(DescriptionProperty),
        typeof(string),
        typeof(CardView),
        null);

    public string Description
    {
        get => (string) GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly BindableProperty CardColorProperty = BindableProperty.Create(
        nameof(CardColorProperty),
        typeof(Color),
        typeof(CardView),
        null);

    public Color CardColor
    {
        get => (Color) GetValue(CardColorProperty);
        set => SetValue(CardColorProperty, value);
    }

    public static readonly BindableProperty ContentColorProperty = BindableProperty.Create(
        nameof(ContentColorProperty),
        typeof(Color),
        typeof(CardView),
        null);

    public Color ContentColor
    {
        get => (Color) GetValue(ContentColorProperty);
        set => SetValue(ContentColorProperty, value);
    }
    #endregion

    #region Private Helpers
    private readonly Border _ContentContainer = new()
    {
        Stroke = Colors.Black,
        StrokeThickness = 1,
        StrokeShape = new RoundRectangle { CornerRadius = 16 },
    };
    private readonly Grid _ContentLayout = new()
    {
        Padding = 8,
        RowDefinitions = Rows.Define(Star, Star),
        ColumnDefinitions = Columns.Define(60, Star),
        RowSpacing = 8,
        ColumnSpacing = 8,
    };
    private readonly Border _ImageContainer = new()
    {
        Stroke = Colors.Transparent,
        StrokeShape = new RoundRectangle { CornerRadius = 16 },
        HeightRequest = 60,
        WidthRequest = 60,
    };
    private readonly Image _Image = new()
    {
        WidthRequest = 30,
        HeightRequest = 30,
    };
    private readonly Label _Title = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalOptions = LayoutOptions.End,
    };
    private readonly Label _Description = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.None,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalOptions = LayoutOptions.Start,
    };
    #endregion

    #region Constructor
    public CardView()
    {
        this.TapGesture(async () =>
        {
            await this.ScaleTo(0.95, 70);
            await this.ScaleTo(1.0, 70);

            Clicked?.Invoke(this, null);
        });

        _ImageContainer.Content = _Image;

        _ContentLayout.Children.Add(_ImageContainer.Row(0).RowSpan(2).Column(0).Center());
        _ContentLayout.Children.Add(_Title.Row(0).Column(1));
        _ContentLayout.Children.Add(_Description.Row(1).Column(1));

        _ContentContainer.Content = _ContentLayout;
        Content = _ContentContainer;
    }
    #endregion

    #region Override
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == ImageSourceProperty.PropertyName)
        {
            _Image.Source = ImageSource;
        }
        else if (propertyName == TitleProperty.PropertyName)
        {
            _Title.Text = Title;
        }
        else if (propertyName == DescriptionProperty.PropertyName)
        {
            _Description.Text = Description;
        }
        else if (propertyName == ImageBackgroundColorProperty.PropertyName)
        {
            _ImageContainer.BackgroundColor = ImageBackgroundColor;
        }
        else if (propertyName == CardColorProperty.PropertyName)
        {
            _ContentContainer.BackgroundColor = CardColor;
        }
        else if (propertyName == ContentColorProperty.PropertyName)
        {
            _Title.TextColor = ContentColor;
            _Description.TextColor = ContentColor;
        }
    }
    #endregion
}
