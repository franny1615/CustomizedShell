using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class StyledEntry : ContentView
{
    #region Events
    public event EventHandler<TextChangedEventArgs> TextChanged;
    #endregion

    #region Public Properties
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(PlaceholderProperty),
        typeof(string),
        typeof(StyledEntry),
        null);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(KeyboardProperty),
        typeof(Keyboard),
        typeof(StyledEntry),
        Keyboard.Plain);

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(TextProperty),
        typeof(string),
        typeof(StyledEntry),
        null);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPasswordProperty),
        typeof(bool),
        typeof(StyledEntry),
        defaultValue: false
    );

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(25, 50),
        RowSpacing = 4,
    };
    private readonly Label _PlaceholderLabel = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalTextAlignment = TextAlignment.Center,
        VerticalOptions = LayoutOptions.Center,
        Margin = new Thickness(16, 0, 0, 0),
        MaxLines = 1
    };
    private readonly Entry _Entry = new()
    {
        FontSize = 20,
        FontAttributes = FontAttributes.None,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalTextAlignment = TextAlignment.Center,
        VerticalOptions = LayoutOptions.Center,
        Keyboard = Keyboard.Plain,
        IsPassword = false,
        Margin = new Thickness(16, 0, 16, 0)
    };
    #endregion

    #region Constructor
    public StyledEntry()
    {
        BindingContext = this;

        _ContentLayout.Children.Add(_PlaceholderLabel.Row(0));
        _ContentLayout.Children.Add(new Border
        {
            Stroke = Colors.DarkGray,
            StrokeShape = new RoundRectangle {  CornerRadius = 16 },
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            Content = _Entry
        }.Row(1));

        _Entry.SetBinding(Entry.TextProperty, nameof(Text));

        Content = _ContentLayout;

        Loaded += EntryLoaded;
        Unloaded += EntryUnloaded;
    }
    #endregion

    #region Overrides
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == PlaceholderProperty.PropertyName)
        {
            _PlaceholderLabel.Text = Placeholder;
        }
        else if (propertyName == KeyboardProperty.PropertyName)
        {
            _Entry.Keyboard = Keyboard;
        }
        else if (propertyName == IsPasswordProperty.PropertyName)
        {
            _Entry.IsPassword = IsPassword;
        }
        else if (propertyName == TextProperty.PropertyName)
        {
            _Entry.Text = Text;
        }
    }
    #endregion

    #region Helpers
    private void EntryUnloaded(object sender, EventArgs e)
    {
        _Entry.TextChanged -= TextChanged;
    }

    private void EntryLoaded(object sender, EventArgs e)
    {
        _Entry.TextChanged += TextChanged;
    }
    #endregion
}
