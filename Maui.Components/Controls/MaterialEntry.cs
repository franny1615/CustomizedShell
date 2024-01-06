using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Controls;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components;

public partial class MaterialEntryModel : ObservableObject
{
    [ObservableProperty]
    public string text = string.Empty;

    [ObservableProperty]
    public string placeholder = string.Empty;

    [ObservableProperty]
    public string placeholderIcon = null;

    [ObservableProperty]
    public bool isSpellCheckEnabled = false;

    [ObservableProperty]
    public bool isPassword = false;

    [ObservableProperty]
    public Keyboard keyboard = Keyboard.Plain;
}

public class MaterialEntry : ContentView
{
    #region Events
    public event EventHandler EntryFocused;
    public event EventHandler EntryUnfocused;
    public event EventHandler<TextChangedEventArgs> TextChanged;
    #endregion

    #region Public Properties
    public static readonly BindableProperty IsDisabledProperty = BindableProperty.Create(
        nameof(IsDisabledProperty),
        typeof(bool),
        typeof(MaterialEntry),
        false
    );

    public bool IsDisabled
    {
        get => (bool)GetValue(IsDisabledProperty);
        set => SetValue(IsDisabledProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly MaterialImage _PlaceholderIcon = new()
    {
        IconSize = 16,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly Label _PlaceholderLabel = new()
    {
        FontSize = 12,
        FontAttributes = FontAttributes.Bold,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly HorizontalStackLayout _PlaceholderContainer = new()
    {
        Spacing = 4,
        Padding = new Thickness(8, 0, 8, 0)
    };
    private readonly Entry _Entry = new()
    {
        FontSize = 18
    };
    private readonly Label _DisabledLabel = new()
    {
        VerticalOptions = LayoutOptions.Center,
        TextColor = Colors.DarkGray,
        FontSize = 18
    };
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(18, Star, Auto),
        RowSpacing = 4
    };
    private readonly Border _EntryBorder = new()
    {
        HeightRequest = 40,
        Stroke = Colors.DarkGray,
        StrokeShape = new RoundRectangle { CornerRadius = 5 },
        Padding = new Thickness(8, 0, 8, 0)
    };
    private readonly HorizontalStackLayout _SupportLayout = new()
    {
        Spacing = 4,
        Padding = new Thickness(8, 0, 8, 0)
    };
    #endregion

    #region Constructor
    public MaterialEntry(MaterialEntryModel model)
    {
        BindingContext = model;

        _DisabledLabel.SetBinding(Label.TextProperty, "Text");

        _Entry.SetBinding(Entry.TextProperty, "Text");
        _Entry.SetBinding(Entry.IsPasswordProperty, "IsPassword");
        _Entry.SetBinding(Entry.IsSpellCheckEnabledProperty, "IsSpellCheckEnabled");
        _Entry.SetBinding(Entry.KeyboardProperty, "Keyboard");
        
        _Entry.TextColor = Application.Current.Resources["TextColor"] as Color;

        _PlaceholderLabel.SetBinding(Label.TextProperty, "Placeholder");
        _PlaceholderIcon.SetBinding(MaterialImage.IconProperty, "PlaceholderIcon");
        _PlaceholderIcon.SetDynamicResource(MaterialImage.IconColorProperty, "TextColor");

        _EntryBorder.Content = _Entry;

        _PlaceholderContainer.Add(_PlaceholderIcon);
        _PlaceholderContainer.Add(_PlaceholderLabel);

        _ContentLayout.Children.Add(_PlaceholderContainer.Row(0));
        _ContentLayout.Children.Add(_EntryBorder.Row(1));
        _ContentLayout.Children.Add(_SupportLayout.Row(2));

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

    #region Helpers
    private void HasFocused(object sender, EventArgs e)
    {
        ShowStatus(null, null, Application.Current.Resources["Primary"] as Color);
        EntryFocused?.Invoke(sender, e);
    }

    private void HasUnfocused(object sender, EventArgs e)
    {
        ShowStatus(null, null, Colors.DarkGray);
        EntryUnfocused?.Invoke(sender, e);
    }

    private void TextHasChanged(object sender, TextChangedEventArgs e)
    {
        TextChanged?.Invoke(sender, e);
    }

    public void ShowStatus(string status, string materialIcon, Color color, bool updateBorder = true)
    {
        _SupportLayout.Clear();

        if (updateBorder)
        {
            _EntryBorder.Stroke = color;
        }

        if (!string.IsNullOrEmpty(materialIcon))
        {
            var icon = new MaterialImage
            {
                Icon = materialIcon,
                IconSize = 12,
                IconColor = color
            };
            _SupportLayout.Add(icon);
        }

        if (!string.IsNullOrEmpty(status))
        {
            var label = new Label
            {
                Text = status,
                TextColor = color,
                FontSize = 12,
                HorizontalTextAlignment = TextAlignment.Start
            };
            _SupportLayout.Add(label);
        }
    }
    #endregion

    #region Overrides
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IsDisabledProperty.PropertyName)
        {
            // TODO: this doesn't work
            if (!IsDisabled)
            {
                _EntryBorder.Content = _Entry;
            }
            else
            {
                _EntryBorder.Content = _DisabledLabel;
            }
        }
    }
    #endregion
}
