using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public enum EntryStyle
{
    Default,
    Search
}

public class MaterialEntry : ContentView
{
    #region Events
    public event EventHandler? EntryFocused;
    public event EventHandler? EntryUnfocused;
    public event EventHandler<TextChangedEventArgs>? TextChanged;
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

    public static readonly BindableProperty IsMultiLineProperty = BindableProperty.Create(
        nameof(IsMultiLine),
        typeof(bool),
        typeof(MaterialEntry),
        false);

    public bool IsMultiLine
    {
        get => (bool)GetValue(IsMultiLineProperty);
        set => SetValue(IsMultiLineProperty, value);
    }

    public static readonly BindableProperty EntryStyleProperty = BindableProperty.Create(
        nameof(EntryStyle),
        typeof(EntryStyle),
        typeof(MaterialEntry),
        EntryStyle.Default
    );

    public EntryStyle EntryStyle
    {
        get => (EntryStyle)GetValue(EntryStyleProperty);
        set => SetValue(EntryStyleProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MaterialEntry),
        null
    );

    public string Text 
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard),
        typeof(Keyboard),
        typeof(MaterialEntry),
        Keyboard.Plain
    );

    public Keyboard Keyboard 
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public static readonly BindableProperty IsSpellCheckEnabledProperty = BindableProperty.Create(
        nameof(IsSpellCheckEnabled),
        typeof(bool),
        typeof(MaterialEntry),
        true
    );

    public bool IsSpellCheckEnabled
    {
        get => (bool)GetValue(IsSpellCheckEnabledProperty);
        set => SetValue(IsSpellCheckEnabledProperty, value);
    }

    public static readonly BindableProperty IsTextPredictionEnabledProperty = BindableProperty.Create(
        nameof(IsTextPredictionEnabled),
        typeof(bool),
        typeof(MaterialEntry),
        true
    );

    public bool IsTextPredictionEnabled
    {
        get => (bool)GetValue(IsTextPredictionEnabledProperty);
        set => SetValue(IsTextPredictionEnabledProperty, value);    
    }

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPassword),
        typeof(bool),
        typeof(MaterialEntry),
        false
    );

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(MaterialEntry),
        null
    );
    
    public string Placeholder   
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty PlaceholderIconProperty = BindableProperty.Create(
        nameof(PlaceholderIcon),
        typeof(string),
        typeof(MaterialEntry),
        null
    );

    public string PlaceholderIcon
    {
        get => (string)GetValue(PlaceholderIconProperty);
        set => SetValue(PlaceholderIconProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new();
    private readonly HorizontalStackLayout _SupportLayout = new HorizontalStackLayout()
        .Spacing(4)
        .Padding(new Thickness(8, 0, 8, 0));
    private readonly HorizontalStackLayout _PlaceholderContainer = new HorizontalStackLayout()
        .Spacing(4)
        .Padding(new Thickness(8, 0, 8, 0));
    private readonly Image _PlaceholderIcon = new Image()
        .CenterVertical();
    private readonly Label _PlaceholderLabel = new Label()
        .FontSize(16)
        .Bold()
        .CenterVertical();
    private readonly Entry _Entry = new Entry()
        .CenterVertical()
        .FontSize(18);
    private readonly Editor _Editor = new Editor()
        .FontSize(18)
        .AutoSize();
    private readonly Label _DisabledLabel = new Label()
        .CenterVertical()
        .TextColor(Colors.DarkGray)
        .FontSize(18);
    private readonly Border _EntryBorder = new Border()
        .Height(DeviceInfo.Current.Platform == DevicePlatform.iOS ? 40 : 50)
        .BorderColor(Colors.DarkGray)
        .BorderThickness(2)
        .BorderShape(new RoundRectangle { CornerRadius = 5 })
        .Padding(new Thickness(8, 0, 8, 0));
    #endregion

    #region Constructor
    public MaterialEntry()
    {
        _PlaceholderLabel.TextColor = Color.FromArgb("#646464");
        _Entry.SetDynamicResource(Entry.TextColorProperty, "TextColor");
        _Editor.SetDynamicResource(Editor.TextColorProperty, "TextColor");
        _EntryBorder.SetDynamicResource(Border.BackgroundColorProperty, "EBGC");
        
        _Entry.Keyboard = Keyboard.Plain; // gets rid of the auto-cap
        _Editor.Keyboard = Keyboard.Plain;

        PlaceContent();

        Loaded += HasLoaded;
        Unloaded += HasUnloaded;
    }

    private void HasLoaded(object? sender, EventArgs e)
    {
        _Entry.Focused += HasFocused;
        _Entry.Unfocused += HasUnfocused;
        _Entry.TextChanged += TextHasChanged;
    }

    private void HasUnloaded(object? sender, EventArgs e)
    {
        _Entry.Focused -= HasFocused;
        _Entry.Unfocused -= HasUnfocused;
        _Entry.TextChanged -= TextHasChanged;
    }
    #endregion

    #region Helpers
    private void HasFocused(object? sender, EventArgs e)
    {
        ShowStatus(null, null, Application.Current?.Resources["Primary"] as Color ?? Colors.DarkGray);
        EntryFocused?.Invoke(sender, e);
    }

    private void HasUnfocused(object? sender, EventArgs e)
    {
        ShowStatus(null, null, Colors.DarkGray);
        EntryUnfocused?.Invoke(sender, e);
    }

    private void TextHasChanged(object? sender, TextChangedEventArgs e)
    {
        Text = e.NewTextValue;
        TextChanged?.Invoke(sender, e);
    }

    public void ShowStatus(string? status, string? materialIcon, Color color, bool updateBorder = true)
    {
        _SupportLayout.Clear();

        if (updateBorder)
        {
            _EntryBorder.Stroke = color;
        }

        if (!string.IsNullOrEmpty(materialIcon))
        {
            var icon = new Image()
                .ApplyMaterialIcon(materialIcon, 16, color);
            _SupportLayout.Add(icon);
        }

        if (!string.IsNullOrEmpty(status))
        {
            var label = new Label
            {
                Text = status,
                TextColor = color,
                FontSize = 14,
                FontAttributes = FontAttributes.None,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Center
            };
            _SupportLayout.Add(label);
        }
    }
    #endregion

    #region Overrides
    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IsDisabledProperty.PropertyName)
        {
            if (!IsDisabled)
            {
                PlaceInput();
            }
            else
            {
                _EntryBorder.Content = _DisabledLabel;
            }
        }
        else if (propertyName == IsMultiLineProperty.PropertyName)
        {
            PlaceInput();
        }
        else if (propertyName == EntryStyleProperty.PropertyName)
        {
            PlaceContent();
        }
        else if (propertyName == TextProperty.PropertyName)
        {
            _Entry.Text = Text;
            _Editor.Text = Text;
        }
        else if (propertyName == KeyboardProperty.PropertyName)
        {
            _Entry.Keyboard = Keyboard;
            _Editor.Keyboard = Keyboard;
        }
        else if (propertyName == IsSpellCheckEnabledProperty.PropertyName)
        {
            _Entry.IsSpellCheckEnabled = IsSpellCheckEnabled;
            _Editor.IsSpellCheckEnabled = IsSpellCheckEnabled;
        }
        else if (propertyName == IsTextPredictionEnabledProperty.PropertyName)  
        {
            _Entry.IsTextPredictionEnabled = IsTextPredictionEnabled;
            _Editor.IsTextPredictionEnabled = IsTextPredictionEnabled;
        }
        else if (propertyName == IsPasswordProperty.PropertyName)
        {
            _Entry.IsPassword = IsPassword;
        }
        else if (propertyName == PlaceholderProperty.PropertyName)
        {
            _PlaceholderLabel.Text = Placeholder;
            _Entry.Placeholder = "";
            _Editor.Placeholder = "";
        }
        else if (propertyName == PlaceholderIconProperty.PropertyName)
        {
            _PlaceholderContainer.Clear();
            if (PlaceholderIcon != null)
            {
                _PlaceholderIcon.ApplyMaterialIcon(PlaceholderIcon, 16, Color.FromArgb("#646464"));
                _PlaceholderContainer.Add(_PlaceholderIcon);
            }
            _PlaceholderContainer.Add(_PlaceholderLabel);
        }
    }

    private void PlaceContent()
    {
        _PlaceholderContainer.Clear();
        _ContentLayout.Clear();
        switch (EntryStyle)
        {
            case EntryStyle.Default:
                _EntryBorder.Content = _Entry;

                _ContentLayout.RowDefinitions = Rows.Define(Auto, Star, Auto);
                _ContentLayout.RowSpacing = 4;

                if (PlaceholderIcon != null) 
                {
                    _PlaceholderContainer.Add(_PlaceholderIcon);
                }
                _PlaceholderContainer.Add(_PlaceholderLabel);

                _ContentLayout.Children.Add(_PlaceholderContainer.Row(0));
                _ContentLayout.Children.Add(_EntryBorder.Row(1));
                _ContentLayout.Children.Add(_SupportLayout.Row(2));

                Content = _ContentLayout;

                break;
            case EntryStyle.Search:
                _ContentLayout.RowDefinitions = Rows.Define(Star, Auto);
                _ContentLayout.ColumnDefinitions = Columns.Define(30, Star);
                _ContentLayout.RowSpacing = 0;
                _ContentLayout.ColumnSpacing = 4;

                _PlaceholderIcon.ApplyMaterialIcon(MaterialIcon.Search, 18, Application.Current!.Resources["TextColor"] as Color ?? Colors.DarkGray);

                _ContentLayout.Children.Add(_PlaceholderIcon.Row(0).Column(0).Center());
                _ContentLayout.Children.Add(_Entry.Row(0).Column(1).CenterVertical());

                _EntryBorder.Content = _ContentLayout;
                Content = _EntryBorder;

                break;
        }
    }

    private void PlaceInput()
    {
        if (IsMultiLine)
        {
            _EntryBorder.Content = _Editor;
            _EntryBorder.HeightRequest = -1;
            _EntryBorder.MinimumHeightRequest = DeviceInfo.Current.Platform == DevicePlatform.iOS ? 40 : 50;
        }
        else
        {
            _EntryBorder.Content = _Entry;
            _EntryBorder.HeightRequest = DeviceInfo.Current.Platform == DevicePlatform.iOS ? 40 : 50;
        }
    }
    #endregion
}
