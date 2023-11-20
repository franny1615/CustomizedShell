using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class SearchBarView : ContentView
{
    #region Events
    public event EventHandler<TextChangedEventArgs> SearchChanged;
    #endregion

    #region Public Properties
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(TextProperty),
        typeof(string),
        typeof(SearchBarView),
        null);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(PlaceholderProperty),
        typeof(string),
        typeof(SearchBarView),
        null);

    public string Placeholder
    {
        get => (string) GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty SearchImageSourceProperty = BindableProperty.Create(
        nameof(SearchImageSourceProperty),
        typeof(ImageSource),
        typeof(SearchBarView),
        null);

    public ImageSource SearchImageSource
    {
        get => (ImageSource)GetValue(SearchImageSourceProperty);
        set { SetValue(SearchImageSourceProperty, value); }
    }

    public static readonly BindableProperty ClearImageSourceProperty = BindableProperty.Create(
        nameof(ClearImageSourceProperty),
        typeof(ImageSource),
        typeof(SearchBarView),
        null);

    public ImageSource ClearImageSource
    {
        get => (ImageSource)GetValue(ClearImageSourceProperty);
        set => SetValue(ClearImageSourceProperty, value);
    }

    public static readonly BindableProperty SearchBackgroundColorProperty = BindableProperty.Create(
        nameof(SearchBackgroundColorProperty),
        typeof(Color),
        typeof(SearchBarView),
        null);

    public Color SearchBackgroundColor
    {
        set => SetValue(SearchBackgroundColorProperty, value);
        get => (Color)GetValue(SearchBackgroundColorProperty);
    }

    public static readonly BindableProperty ContentColorProperty = BindableProperty.Create(
        nameof(ContentColorProperty),
        typeof(Color),
        typeof(SearchBarView),
        null);

    public Color ContentColor
    {
        get => (Color) GetValue(ContentColorProperty);
        set => SetValue(ContentColorProperty, value);
    }
    #endregion

    #region Private Varibles
    private readonly Border _ContentContainer = new()
    {
        Stroke = Colors.Transparent,
        StrokeShape = new RoundRectangle { CornerRadius = 16 },
        Padding = 8
    };
    private readonly Grid _ContentLayout = new()
    {
        ColumnDefinitions = Columns.Define(30, Star, 30),
        ColumnSpacing = 8
    };
    private readonly Image _SearchIcon = new()
    {
        HeightRequest = 30,
        WidthRequest = 30,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly Image _ClearIcon = new()
    {
        HeightRequest = 30,
        WidthRequest = 30,
        VerticalOptions = LayoutOptions.Center,
        Opacity = 0
    };
    private readonly Entry _SearchEntry = new()
    {
        FontSize = 16,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly IconTintColorBehavior _TintBehavior = new(); 
    #endregion

    #region Constructors
    public SearchBarView()
    {
        BindingContext = this;

        _SearchEntry.SetBinding(Entry.TextProperty, nameof(Text));
        _SearchEntry.TextChanged += EntryChanged;

        _SearchIcon.Behaviors.Add(_TintBehavior);
        _ClearIcon.Behaviors.Add(_TintBehavior);

        _ClearIcon.TapGesture(() => _SearchEntry.Text = "");

        _ContentLayout.Children.Add(_SearchIcon.Column(0));
        _ContentLayout.Children.Add(_SearchEntry.Column(1));
        _ContentLayout.Children.Add(_ClearIcon.Column(2));

        _ContentContainer.Content = _ContentLayout;
        Content = _ContentContainer;
    }

    private void EntryChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length > 0)
        {
            _ClearIcon.Opacity = 1;
        }
        else
        {
            _ClearIcon.Opacity = 0;
        }
        Text = e.NewTextValue;
        SearchChanged?.Invoke(this, e);
    }
    #endregion

    #region Overrides
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == ContentColorProperty.PropertyName)
        {
            _TintBehavior.TintColor = ContentColor;
            _SearchEntry.TextColor = ContentColor;
            _SearchEntry.PlaceholderColor = ContentColor.WithAlpha(0.6f);
        }
        else if (propertyName == PlaceholderProperty.PropertyName)
        {
            _SearchEntry.Placeholder = Placeholder;
        }
        else if (propertyName == TextProperty.PropertyName)
        {
            _SearchEntry.Text = Text;
        }
        else if (propertyName == SearchImageSourceProperty.PropertyName) 
        {
            _SearchIcon.Source = SearchImageSource;
        }
        else if (propertyName == ClearImageSourceProperty.PropertyName)
        {
            _ClearIcon.Source = ClearImageSource;
        }
        else if (propertyName == SearchBackgroundColorProperty.PropertyName)
        {
            _ContentContainer.BackgroundColor = SearchBackgroundColor;
        }
    }
    #endregion
}
