using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using SQLite;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class StyledEntry : ContentView
{
    #region Events
    public event EventHandler<TextChangedEventArgs> TextChanged;
    public event EventHandler ActionClicked;
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

    public static readonly BindableProperty StatusIconProperty = BindableProperty.Create(
        nameof(StatusIconProperty),
        typeof(ImageSource),
        typeof(StyledEntry),
        null
    );

    public ImageSource StatusIcon
    {
        get => (ImageSource)GetValue(StatusIconProperty);
        set => SetValue(StatusIconProperty, value);
    }

    public static readonly BindableProperty StatusTextProperty = BindableProperty.Create(
        nameof(StatusTextProperty),
        typeof(string),
        typeof(StyledEntry),
        null
    );

    public string StatusText 
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public static readonly BindableProperty StatusColorProperty = BindableProperty.Create(
        nameof(StatusColorProperty),
        typeof(Color),
        typeof(StyledEntry),
        null
    );

    public Color StatusColor 
    {
        get => (Color)GetValue(StatusColorProperty);
        set => SetValue(StatusColorProperty, value);
    }

    public static readonly BindableProperty ActionIconProperty = BindableProperty.Create(
        nameof(ActionIconProperty),
        typeof(ImageSource),
        typeof(StyledEntry),
        null
    );

    public ImageSource ActionIcon
    {
        get => (ImageSource) GetValue(ActionIconProperty);
        set => SetValue(ActionIconProperty, value);
    }

    public Entry TextInput
    {
        get
        {
            return _Entry;
        }
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(25, 50, Auto),
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
    private readonly Border _EntryContainer = new()
    {
        Stroke = Colors.Black,
        StrokeThickness = 1,
        StrokeShape = new RoundRectangle {  CornerRadius = 16 },
        BackgroundColor = Colors.Transparent,
        Padding = 0,
    };
    private readonly Grid _EntryLayout = new()
    {
        ColumnSpacing = 8,
        ColumnDefinitions = Columns.Define(Star, 30)
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
    private readonly Image _ActionImage = new()
    {
        HeightRequest = 25,
        WidthRequest = 25
    };
    private readonly HorizontalStackLayout _StatusContainer = new() 
    { 
        Spacing = 4,
        Margin = new Thickness(16, 0, 16, 0)
    };
    private readonly Image _StatusImage = new()
    {
        HeightRequest = 15,
        WidthRequest = 15,
    };
    private readonly Label _StatusLabel = new()
    {
        FontSize = 12,
        FontAttributes = FontAttributes.Bold
    };
    private readonly IconTintColorBehavior _StatusColor = new();
    #endregion

    #region Constructor
    public StyledEntry()
    {
        BindingContext = this;

        _EntryContainer.Content = _Entry;
        _EntryContainer.SetAppThemeColor(
            Entry.BackgroundColorProperty, 
            Application.Current.Resources["CardColorLight"] as Color,
            Application.Current.Resources["CardColorDark"] as Color);
        
        _EntryLayout.Children.Add(_EntryContainer.Column(0).ColumnSpan(2));

        _ContentLayout.Children.Add(_PlaceholderLabel.Row(0));
        _ContentLayout.Children.Add(_EntryLayout.Row(1));

        _ActionImage.TapGesture(async () => 
        {
            await _ActionImage.ScaleTo(0.95, 70);
            await _ActionImage.ScaleTo(1.0, 70);

            ActionClicked?.Invoke(this, null);
        });

        _Entry.SetBinding(Entry.TextProperty, nameof(Text));

        Content = _ContentLayout;

        _StatusImage.Behaviors.Add(_StatusColor);
        _StatusContainer.Children.Add(_StatusImage.Center());
        _StatusContainer.Children.Add(_StatusLabel.CenterVertical().Start());

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
        else if (propertyName == StatusColorProperty.PropertyName)
        {
            _EntryContainer.Stroke = StatusColor;
            _StatusColor.TintColor = StatusColor; 
            _StatusLabel.TextColor = StatusColor;
        }
        else if (propertyName == StatusIconProperty.PropertyName ||
                 propertyName == StatusTextProperty.PropertyName)
        {
            _ContentLayout.Children.Remove(_StatusContainer);
            if (StatusIcon != null && !string.IsNullOrEmpty(StatusText))
            {
                _StatusImage.Source = StatusIcon;
                _StatusLabel.Text = StatusText;
                _ContentLayout.Children.Add(_StatusContainer.Row(2).Start());
            }
        } 
        else if (propertyName == ActionIconProperty.PropertyName)
        {
            _EntryLayout.Children.Remove(_ActionImage);
            if (ActionIcon != null)
            {
                _ActionImage.Source = ActionIcon;
                _EntryContainer.ColumnSpan(1);
                _EntryLayout.Children.Add(_ActionImage.Column(1));
            }
            else
            {
                _EntryContainer.ColumnSpan(2);
            }
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
