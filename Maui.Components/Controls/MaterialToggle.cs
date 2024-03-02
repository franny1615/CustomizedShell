using CommunityToolkit.Maui.Markup;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class MaterialToggle : ContentView
{
    #region Events
    public event EventHandler<ToggledEventArgs> Toggled;
    #endregion

    #region Public Properties
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(string),
        typeof(MaterialToggle),
        null);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MaterialToggle),
        null);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
        nameof(IsToggled),
        typeof(bool),
        typeof(MaterialToggle),
        false);

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(MaterialToggle),
        null);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColor),
        typeof(Color),
        typeof(MaterialToggle),
        null);

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new() { ColumnDefinitions = Columns.Define(30, Star, Auto), ColumnSpacing = 8 };
    private readonly MaterialImage _Icon = new() { IconSize = 25, };
    private readonly Label _Text = new() { FontSize = 16, FontAttributes = FontAttributes.None, HorizontalOptions = LayoutOptions.Start };
    private readonly Switch _Switch = new() { ThumbColor = Application.Current.Resources["Primary"] as Color };
    #endregion

    #region Constructor
    public MaterialToggle()
    {
        _ContentLayout.Add(_Icon.Column(0).Center());
        _ContentLayout.Add(_Text.Column(1).Start().CenterVertical());
        _ContentLayout.Add(_Switch.Column(2).Center());

        Content = _ContentLayout;

        _Switch.Toggled += ToggledLocal;
    }
    ~MaterialToggle()
    {
        _Switch.Toggled -= ToggledLocal; 
    }
    #endregion

    #region Helpers
    private void ToggledLocal(object sender, ToggledEventArgs e)
    {
        IsToggled = e.Value;
        Toggled?.Invoke(sender, e);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == IconProperty.PropertyName)
        {
            _Icon.Icon = Icon;
        }
        else if (propertyName == TextProperty.PropertyName)
        {
            _Text.Text = Text;
        }
        else if (propertyName == IsToggledProperty.PropertyName)
        {
            _Switch.IsToggled = IsToggled;
        }
        else if (propertyName == IconColorProperty.PropertyName)
        {
            _Icon.IconColor = IconColor;
        }
        else if (propertyName == TextColorProperty.PropertyName)
        {
            _Text.TextColor = TextColor;
        }
    }
    #endregion
}
