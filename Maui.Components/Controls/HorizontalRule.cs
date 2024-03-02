using CommunityToolkit.Maui.Markup;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class HorizontalRule : Grid
{
    #region Public Properties
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(HorizontalRule),
        null);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Label _Text = new() { FontSize = 16, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center };
    #endregion

    #region Constructor
    public HorizontalRule()
    {
        ColumnDefinitions = Columns.Define(Star, Auto, Star);
        ColumnSpacing = 8;

        Children.Add(new BoxView
        {
            HeightRequest = 1,
            Color = Application.Current.Resources["Primary"] as Color
        }.Column(0));
        Children.Add(_Text.Column(1));
        Children.Add(new BoxView
        {
            HeightRequest = 1,
            Color = Application.Current.Resources["Primary"] as Color
        }.Column(2));
    }
    #endregion

    #region Override
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == TextProperty.PropertyName) 
        {
            _Text.Text = Text;
        }
    }
    #endregion
}
