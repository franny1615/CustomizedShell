using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class MaterialCardView : Border
{
    #region Public Events
    public event EventHandler Clicked;
    #endregion

    #region Public Properties 
    public static readonly BindableProperty HeadlineProperty = BindableProperty.Create(
        nameof(HeadlineProperty),
        typeof(string),
        typeof(MaterialCardView),
        null);

    public string Headline
    {
        get => (string)GetValue(HeadlineProperty);
        set => SetValue(HeadlineProperty, value);
    }

    public static readonly BindableProperty SupportingTextProperty = BindableProperty.Create(
        nameof(SupportingTextProperty),
        typeof(string),
        typeof(MaterialCardView),
        null);

    public string SupportingText
    {
        get => (string)GetValue(SupportingTextProperty);
        set => SetValue(SupportingTextProperty, value);
    }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(IconProperty),
        typeof(string),
        typeof(MaterialCardView),
        null);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColorProperty),
        typeof(Color),
        typeof(MaterialCardView),
        null);

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public static readonly BindableProperty TrailingIconProperty = BindableProperty.Create(
        nameof(TrailingIconProperty),
        typeof(string),
        typeof(MaterialCardView),
        null);

    public string TrailingIcon
    {
        get => (string)GetValue(TrailingIconProperty);
        set => SetValue(TrailingIconProperty, value);
    }

    public static readonly BindableProperty TrailingIconColorProperty = BindableProperty.Create(
        nameof(TrailingIconColorProperty),
        typeof(Color),
        typeof(MaterialCardView),
        null);

    public Color TrailingIconColor
    {
        get => (Color)GetValue(TrailingIconColorProperty);
        set => SetValue (TrailingIconColorProperty, value);
    }

    public static readonly BindableProperty TrailingIconIsVisibleProperty = BindableProperty.Create(
        nameof(TrailingIconIsVisible),
        typeof(bool),
        typeof(MaterialCardView),
        true);

    public bool TrailingIconIsVisible
    {
        get => (bool)GetValue(TrailingIconIsVisibleProperty);
        set => SetValue(TrailingIconIsVisibleProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColorProperty),
        typeof(Color),
        typeof(MaterialCardView),
        null);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new()
    {
        Padding = 8,
        RowDefinitions = Rows.Define(Star, Star),
        ColumnDefinitions = Columns.Define(30, Star, 30),
        ColumnSpacing = 8,
        RowSpacing = 8,
    };
    private readonly Label _Headline = new()
    {
        FontSize = 16, 
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalOptions = LayoutOptions.Center,
        MaxLines = 1,
    };
    private readonly Label _SupportingText = new()
    {
        FontSize = 14,
        FontAttributes = FontAttributes.None,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalOptions = LayoutOptions.Center,
        MaxLines = 1,
    };
    private readonly MaterialImage _Icon = new()
    {
        IconSize = 30,
    };
    private readonly MaterialImage _TrailingIcon = new()
    {
        IconSize = 30,
    };
    #endregion

    #region Constructor
    public MaterialCardView()
    {
        _ContentLayout.Children.Add(_Icon.Row(0).Column(0).RowSpan(2).Center());
        _ContentLayout.Children.Add(_Headline.Row(0).Column(1));
        _ContentLayout.Children.Add(_SupportingText.Row(1).Column(1));
        _ContentLayout.Children.Add(_TrailingIcon.Row(0).Column(2).RowSpan(2).Center());

        this.TapGesture(async () =>
        {
            await this.ScaleTo(0.95, 70);
            await this.ScaleTo(1.0, 70);

            Clicked?.Invoke(this, null);
        });

        Padding = 0;
        Margin = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        Content = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == HeadlineProperty.PropertyName)
        {
            _Headline.Text = Headline;
        }
        else if (propertyName == SupportingTextProperty.PropertyName)
        {
            _SupportingText.Text = SupportingText;
            if (string.IsNullOrEmpty(SupportingText))
            {
                _Headline.RowSpan(2).CenterVertical();
            }
            else 
            {
                _Headline.RowSpan(1);
            }
        }
        else if (propertyName == IconProperty.PropertyName)
        {
            _Icon.Icon = Icon;
        }
        else if (propertyName == IconColorProperty.PropertyName)
        {
            _Icon.IconColor = IconColor;
        }
        else if (propertyName == TrailingIconProperty.PropertyName)
        {
            _TrailingIcon.Icon = TrailingIcon;
        }
        else if (propertyName == TrailingIconColorProperty.PropertyName)
        {
            _TrailingIcon.IconColor = TrailingIconColor;
        }
        else if (propertyName == TextColorProperty.PropertyName)
        {
            _Headline.TextColor = TextColor;
            _SupportingText.TextColor = TextColor;  
        }
        else if (propertyName == TrailingIconIsVisibleProperty.PropertyName)
        {
            _TrailingIcon.IsVisible = TrailingIconIsVisible;
        }
    }
    #endregion
}
