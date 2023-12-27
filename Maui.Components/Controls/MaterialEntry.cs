using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components;

public class MaterialEntry : ContentView
{
    #region Events
    public event EventHandler EntryFocused;
    public event EventHandler EntryUnfocused;
    public event EventHandler<TextChangedEventArgs> TextChanged;
    #endregion

    #region Public Properties
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(PlaceholderProperty),
        typeof(string),
        typeof(MaterialEntry),
        null
    );

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(TextProperty),
        typeof(string),
        typeof(MaterialEntry),
        null
    );

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty SupportingTextProperty = BindableProperty.Create(
        nameof(SupportingTextProperty),
        typeof(string),
        typeof(MaterialEntry),
        null
    );

    public string SupportingText
    {
        get => (string)GetValue(SupportingTextProperty);
        set => SetValue(SupportingTextProperty, value);
    }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(KeyboardProperty),
        typeof(Keyboard),
        typeof(MaterialEntry),
        Keyboard.Plain
    );

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    } 

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPasswordProperty),
        typeof(bool),
        typeof(MaterialEntry),
        false
    );

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColorProperty),
        typeof(Color),
        typeof(MaterialEntry),
        null
    );

    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public static readonly BindableProperty IsSpellCheckEnabledProperty = BindableProperty.Create(
        nameof(IsSpellCheckEnabledProperty),
        typeof(bool),
        typeof(MaterialEntry),
        false
    );

    public bool IsSpellCheckEnabled
    {
        get => (bool)GetValue(IsSpellCheckEnabledProperty);
        set => SetValue(IsSpellCheckEnabledProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Label _PlaceholderLabel = new()
    {
        FontSize = 12,
        FontAttributes = FontAttributes.Bold,
        Padding = new Thickness(8, 0, 8, 0)
    };
    private readonly Label _SupportingLabel = new()
    {
        FontSize = 12,
        Padding = new Thickness(8, 0, 8, 0)
    };
    private readonly Entry _Entry = new()
    {
        FontSize = 18
    };
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(18, Star, Auto),
        RowSpacing = 4
    };
    private readonly Border _EntryBorder = new()
    {
        Stroke = Colors.DarkGray,
        StrokeShape = new RoundRectangle { CornerRadius = 5 },
        Padding = new Thickness(8, 0, 8, 0)
    };
    #endregion

    #region Constructor
    public MaterialEntry()
    {
        BindingContext = this;

        _EntryBorder.Content = _Entry;

        _ContentLayout.Children.Add(_PlaceholderLabel.Row(0));
        _ContentLayout.Children.Add(_EntryBorder.Row(1));
        _ContentLayout.Children.Add(_SupportingLabel.Row(2));

        Content = _ContentLayout;

        Loaded += HasLoaded;
        Unloaded += HasUnloaded;
    }

    private void HasLoaded(object sender, EventArgs e)
    {
        _Entry.Focused += HasFocused;
        _Entry.Unfocused += HasUnfocused;
        _Entry.TextChanged += TextHasChanged;
    }

    private void HasUnloaded(object sender, EventArgs e)
    {
        _Entry.Focused -= HasFocused;
        _Entry.Unfocused -= HasUnfocused;
        _Entry.TextChanged -= TextHasChanged;
    }
    #endregion

    #region Overrides
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TextProperty.PropertyName)
        {
            _Entry.Text = Text;
        }
        else if (propertyName == PlaceholderProperty.PropertyName)
        {
            _PlaceholderLabel.Text = Placeholder;
        }
        else if (propertyName == SupportingTextProperty.PropertyName)
        {
            _SupportingLabel.Text = SupportingText;
        }
        else if (propertyName == IsPasswordProperty.PropertyName)
        {
            _Entry.IsPassword = IsPassword;
        }
        else if (propertyName == KeyboardProperty.PropertyName)
        {
            _Entry.Keyboard = Keyboard;
        }
        else if (propertyName == BorderColorProperty.PropertyName)
        {
            _EntryBorder.Stroke = BorderColor;
        }
        else if (propertyName == IsSpellCheckEnabledProperty.PropertyName)
        {
            _Entry.IsSpellCheckEnabled = IsSpellCheckEnabled;
        }
    }
    #endregion

    #region Helpers
    private void HasFocused(object sender, EventArgs e)
    {
        _EntryBorder.Stroke = Application.Current.Resources["Primary"] as Color;
        EntryFocused?.Invoke(sender, e);
    }

    private void HasUnfocused(object sender, EventArgs e)
    {
        _EntryBorder.Stroke = Colors.DarkGray;
        EntryUnfocused?.Invoke(sender, e);
    }

    private void TextHasChanged(object sender, TextChangedEventArgs e)
    {
        Text = e.NewTextValue;
        TextChanged?.Invoke(sender, e);
    }
    #endregion
}
