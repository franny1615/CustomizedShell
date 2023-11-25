using CommunityToolkit.Maui.Markup;
using Maui.Components.Controls;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using Maui.Components.Utilities;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Pages;

public class EditEventArgs : EventArgs
{
    public ISearchable Item { get; set; }
}

public class SearchPage : BasePage
{
    #region Events
    public event EventHandler<EditEventArgs> Add;
    public event EventHandler<EditEventArgs> Edit;
    #endregion

    #region Private Properties
    private Debouncer _Debouncer = new(0.5);
    private readonly ILanguageService _LanguageService;
    private ISearchViewModel _SearchViewModel => (ISearchViewModel)BindingContext;
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Auto, Star),
        RowSpacing = 16,
        Padding = 16
    };
    private readonly SearchBarView _Search = new();
    private readonly CollectionView _Collection = new()
    {
        ZIndex = 0,
        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 16 },
    };
    private readonly FloatingActionButton _Add = new()
    {
        ZIndex = 1,
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        FABStyle = FloatingActionButtonStyle.Regular
    };
    #endregion

    #region Constructor
    public SearchPage(
        ILanguageService languageService,
        ISearchViewModel searchViewModel) : base(languageService)
    {
        _LanguageService = languageService;

        BindingContext = searchViewModel;

        Title = searchViewModel.PageTitle;

        _Search.SetAppThemeColor(
            SearchBarView.SearchBackgroundColorProperty, 
            Application.Current.Resources["CardColorLight"] as Color, 
            Application.Current.Resources["CardColorDark"] as Color);
        _Search.SetAppThemeColor(SearchBarView.ContentColorProperty, Colors.Black, Colors.White);
        _Search.Placeholder = _SearchViewModel.SearchPlaceholder;
        _Search.ClearImageSource = _SearchViewModel.ClearSearchIcon;
        _Search.SearchImageSource = _SearchViewModel.SearchIcon;

        _ContentLayout.Children.Add(_Search.Row(0));
        _ContentLayout.Children.Add(_Collection.Row(1));
        if (_SearchViewModel.ShowAdd)
        {
            _ContentLayout.Children.Add(_Add.Row(1).End().Bottom());
        }

        _Add.ImageSource = _SearchViewModel.AddIcon;

        _Collection.SetBinding(CollectionView.ItemsSourceProperty, "Items");
        _Collection.ItemTemplate = new DataTemplate(() => 
        {
            switch (_SearchViewModel.CardStyle)
            {
                case CardStyle.Mini:
                    var mini = new MiniCardView();
                    mini.Clicked += EditClicked;
                    mini.SetBinding(MiniCardView.BindingContextProperty, ".");
                    mini.SetBinding(MiniCardView.TitleProperty, "Name");
                    mini.SetBinding(MiniCardView.ImageSourceProperty, "Icon");
                    mini.SetBinding(MiniCardView.ImageBackgroundColorProperty, "IconBackgroundColor");
                    mini.SetAppThemeColor(
                        MiniCardView.CardColorProperty,
                        Application.Current.Resources["CardColorLight"] as Color, 
                        Application.Current.Resources["CardColorDark"] as Color);
                    
                    return mini;
                case CardStyle.Regular:
                    var regular = new CardView();
                    regular.Clicked += EditClicked;
                    regular.SetBinding(CardView.BindingContextProperty, ".");
                    regular.SetBinding(CardView.TitleProperty, "Name");
                    regular.SetBinding(CardView.DescriptionProperty, "Description");
                    regular.SetBinding(CardView.ImageSourceProperty, "Icon");
                    regular.SetAppThemeColor(
                        CardView.CardColorProperty,
                        Application.Current.Resources["CardColorLight"] as Color, 
                        Application.Current.Resources["CardColorDark"] as Color);
                    regular.SetBinding(CardView.ImageBackgroundColorProperty, "IconBackgroundColor");
                    
                    return regular;
                default:
                    return new Grid();
            }
        });

        Content = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _SearchViewModel.GetAllItems(_Search.Text);
        _Add.Clicked += AddClicked;
        _Search.SearchChanged += SearchChanged;
    }

    protected override void OnDisappearing()
    {
        _Add.Clicked -= AddClicked;
        _Search.SearchChanged -= SearchChanged;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private void AddClicked(object sender, EventArgs e)
    {
        Add?.Invoke(this, new EditEventArgs
        {
            Item = _SearchViewModel.NewSearchable()
        });
    }

    private void EditClicked(object sender, EventArgs e)
    {
        if (sender is MiniCardView mini && 
            mini.BindingContext is ISearchable miniItem)
        {
            Edit?.Invoke(this, new EditEventArgs
            {
                Item = miniItem
            });
        }
        else if (sender is CardView card && 
                 card.BindingContext is ISearchable cardItem)
        {
            Edit?.Invoke(this, new EditEventArgs
            {
                Item = cardItem
            });
        }
    }

    private void SearchChanged(object sender, TextChangedEventArgs e)
    {
        _Debouncer.Debounce(async () => 
        {
            await _SearchViewModel.GetAllItems(_Search.Text);
        });
    }
    #endregion
}
