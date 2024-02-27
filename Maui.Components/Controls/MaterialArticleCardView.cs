using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public class MaterialArticleCardView : Border
{
    #region Events
    public event EventHandler Clicked;
    #endregion

    #region Public Properties
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(MaterialArticleCardView),
        null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty ArticleProperty = BindableProperty.Create(
        nameof(Article),
        typeof(string),
        typeof(MaterialArticleCardView),
        null);

    public string Article
    {
        get => (string)GetValue(ArticleProperty);
        set => SetValue(ArticleProperty, value);
    }

    public static readonly BindableProperty MainSupportOneProperty = BindableProperty.Create(
        nameof(MainSupportOne),
        typeof(string),
        typeof(MaterialArticleCardView),
        null);

    public string MainSupportOne
    {
        get => (string)GetValue(MainSupportOneProperty);
        set => SetValue(MainSupportOneProperty, value);
    }

    public static readonly BindableProperty MainSupportTwoProperty = BindableProperty.Create(
        nameof(MainSupportTwo),
        typeof(string),
        typeof(MaterialArticleCardView),
        null);

    public string MainSupportTwo
    {
        get => (string)GetValue(MainSupportTwoProperty);
        set => SetValue(MainSupportTwoProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(MaterialArticleCardView),
        Colors.Transparent);

    public Color TextColor
    {
        get => (Color)(GetValue(TextColorProperty));
        set => SetValue(TextColorProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(24, Star, 24),
        ColumnDefinitions = Columns.Define(Star, Star),
        Padding = 8,
        ColumnSpacing = 4,
        RowSpacing = 4
    };
    private readonly Label _Article = new()
    {
        FontSize = 12,
        FontAttributes = FontAttributes.None,
        HorizontalTextAlignment = TextAlignment.Start
    };
    private readonly Label _MainOne = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start,
    };
    private readonly Label _MainTwo = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.End,
    };
    private readonly Label _Title = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start,
    };
    #endregion

    #region Constructor
    public MaterialArticleCardView()
    {
        Padding = 0;
        Margin = 0;
        StrokeShape = new RoundRectangle { CornerRadius = 5 };

        this.TapGesture(async () =>
        {
            await this.ScaleTo(0.95, 70);
            await this.ScaleTo(1.0, 70);

            Clicked?.Invoke(this, null);
        });

        _ContentLayout.Add(_Title.Row(0).Column(0));
        _ContentLayout.Add(_Article.Row(1).Column(0).ColumnSpan(2));
        _ContentLayout.Add(_MainOne.Row(2).Column(0));
        _ContentLayout.Add(_MainTwo.Row(2).Column(1));

        Content = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == ArticleProperty.PropertyName)
        {
            _Article.Text = Article;
        }
        else if (propertyName == MainSupportOneProperty.PropertyName)
        {
            _MainOne.Text = MainSupportOne;
        }
        else if (propertyName == MainSupportTwoProperty.PropertyName)
        {
            _MainTwo.Text = MainSupportTwo;
        }
        else if (propertyName == TitleProperty.PropertyName)
        {
            _Title.Text = Title;
        }
        else if (propertyName == TextColorProperty.PropertyName)
        {
            _Title.TextColor = TextColor;
            _Article.TextColor = TextColor;
            _MainOne.TextColor = TextColor;
            _MainTwo.TextColor = TextColor;
        }
    }
    #endregion
}
