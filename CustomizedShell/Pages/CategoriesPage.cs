using CommunityToolkit.Maui.Markup;
using CustomizedShell.Models;
using CustomizedShell.Services;
using CustomizedShell.ViewModels;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace CustomizedShell.Pages;

public class CategoriesPage : SearchPage
{
    #region Private Variables
    private DataViewModel _DataViewModel => (DataViewModel)BindingContext;
    private readonly Debouncer _SearchDebouncer = new();
    private readonly Color _Primary = Application.Current.Resources["Primary"] as Color;
    private readonly Color _CardLight = Application.Current.Resources["CardColorLight"] as Color;
    private readonly Color _CardDark = Application.Current.Resources["CardColorDark"] as Color;
    private readonly SearchBarView _Search = new()
    {
        SearchImageSource = "search.png",
        ClearImageSource = "close.png",
        Placeholder = LanguageService.Instance.StringForKey("Search")
    };
    private readonly CollectionView _CategoriesCollection = new()
    {
        ZIndex = 0,
        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 16 },
    };
    private readonly FloatingActionButton _AddCategory = new()
    {
        ZIndex = 1,
        ImageSource = "add.png",
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        FABStyle = FloatingActionButtonStyle.Regular
    };
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Auto, Star),
        RowSpacing = 16,
        Padding = 16
    };
    #endregion

    #region Constructors
    public CategoriesPage(DataViewModel dataViewModel)
    {
        OverrideBackButtonText();

        BindingContext = dataViewModel;

        Title = Lang.StringForKey("Categories");

        _Search.SetAppThemeColor(SearchBarView.SearchBackgroundColorProperty, _CardLight, _CardDark);
        _Search.SetAppThemeColor(SearchBarView.ContentColorProperty, Colors.Black, Colors.White);

        _ContentLayout.Children.Add(_Search.Row(0));
        _ContentLayout.Children.Add(_CategoriesCollection.Row(1));
        _ContentLayout.Children.Add(_AddCategory.Row(1).End().Bottom());

        _CategoriesCollection.SetBinding(CollectionView.ItemsSourceProperty, "Categories");
        _CategoriesCollection.ItemTemplate = new DataTemplate(() => 
        {
            var view = new MiniCardView();
            view.Clicked += EditCategory;
            view.BindingContext = new Binding(".");
            view.SetBinding(MiniCardView.TitleProperty, "Name");
            view.SetAppThemeColor(MiniCardView.CardColorProperty, _CardLight, _CardDark);
            view.ImageBackgroundColor = _Primary;
            view.ImageSource = "category.png";

            return view;
        });

        Content = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.SearchChanged += SearchChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _Search.SearchChanged -= SearchChanged;
    }
    #endregion

    #region Helpers
    private void EditCategory(object sender, EventArgs e)
    {
        if (sender is MiniCardView view &&
            view.BindingContext is Category status)
        {
            // TODO: show popup
        }
    }

    private void SearchChanged(object sender, TextChangedEventArgs e)
    {
        _SearchDebouncer.Debounce(() =>
        {

        });
    }
    #endregion
}
