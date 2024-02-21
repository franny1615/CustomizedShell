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

    public static readonly BindableProperty SecondarySupportOneProperty = BindableProperty.Create(
        nameof(SecondarySupportOne),
        typeof(string),
        typeof(MaterialArticleCardView),
        null);

    public string SecondarySupportOne
    {
        get => (string)(GetValue(SecondarySupportOneProperty));
        set => SetValue(SecondarySupportOneProperty, value);
    }

    public static readonly BindableProperty SecondarySupportTwoProperty = BindableProperty.Create(
        nameof(SecondarySupportTwo),
        typeof(string),
        typeof(MaterialArticleCardView),
        null);

    public string SecondarySupportTwo
    {
        get => (string)(GetValue(SecondarySupportTwoProperty));
        set => SetValue(SecondarySupportTwoProperty, value);
    }
    #endregion

    #region Private Properties
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Star, Auto, Auto),
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
    private readonly Label _SecondOne = new()
    {
        FontSize = 12,
        FontAttributes = FontAttributes.None,
        HorizontalTextAlignment = TextAlignment.Start,
    };
    private readonly Label _SecondTwo = new()
    {
        FontSize = 12,
        FontAttributes = FontAttributes.None,
        HorizontalTextAlignment = TextAlignment.End,
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

        _ContentLayout.Add(_Article.Row(0).Column(0).ColumnSpan(2));
        _ContentLayout.Add(_MainOne.Row(1).Column(0));
        _ContentLayout.Add(_MainTwo.Row(1).Column(1));
        _ContentLayout.Add(_SecondOne.Row(2).Column(0));
        _ContentLayout.Add(_SecondTwo.Row(2).Column(1));

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
        else if (propertyName == SecondarySupportOneProperty.PropertyName)
        {
            _SecondOne.Text = SecondarySupportOne;
        }
        else if (propertyName == SecondarySupportTwoProperty.PropertyName)
        {
            _SecondTwo.Text = SecondarySupportTwo;
        }
    }
    #endregion
}
