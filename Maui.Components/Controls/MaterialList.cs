using CommunityToolkit.Maui.Markup;
using Maui.Components.Utilities;
using System.Collections.ObjectModel;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Controls;

public interface IMaterialListVM<T>
{
    public int ItemsPerPage { get; set; }
    public MaterialPaginationModel PaginationModel { get; set; }
    public MaterialEntryModel SearchModel { get; set; }
    public ObservableCollection<T> Items { get; set; }
    public Task GetItems();
}

public class MaterialList<T> : ContentView
{
    #region Events
    public event EventHandler<ClickedEventArgs> AddItemClicked;
    public event EventHandler FetchedNewPage;
    #endregion

    #region Private Properties
    private IMaterialListVM<T> _ViewModel => (IMaterialListVM<T>) BindingContext;
    private readonly Debouncer _SearchDebouncer = new(1.25);
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Auto, Star, Auto),
        RowSpacing = 8,
        Padding = 16
    };
    private readonly RefreshView _Refresh = new();
    private readonly CollectionView _Collection = new()
    {
        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 8 },
        ZIndex = 0,
    };
    private readonly MaterialEntry _Search;
    private readonly MaterialPagination _Pagination;
    private readonly MaterialImage _NoItemIcon = new()
    {
        Icon = MaterialIcon.Check_circle,
        IconColor = Application.Current.Resources["TextColor"] as Color,
        IconSize = 40
    };
    private readonly Label _NoItems = new()
    {
        FontSize = 21,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
    };
    private readonly VerticalStackLayout _NoItemsUI = new()
    {
        Spacing = 8,
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Center
    };
    private readonly FloatingActionButton _Add = new()
    {
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Add, Colors.White),
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        FABStyle = FloatingActionButtonStyle.Regular,
        Margin = new Thickness(0, 0, 0, 0)
    };
    #endregion

    #region Constructor
    public MaterialList(
        string noItemsText,
        string noItemsIcon,
        DataTemplate cardTemplate,
        IMaterialListVM<T> viewModel,
        bool isEditable = false)
    {
        _NoItemIcon.Icon = noItemsIcon;
        _NoItems.Text = noItemsText;
        _NoItemsUI.Add(_NoItemIcon.Center());
        _NoItemsUI.Add(_NoItems);

        BindingContext = viewModel;

        _Search = new(viewModel.SearchModel);
        _Pagination = new(viewModel.PaginationModel);

        _Pagination.ContentColor = Colors.White;
        _Pagination.BackgroundColor = Application.Current.Resources["Primary"] as Color;

        _Collection.SetBinding(CollectionView.ItemsSourceProperty, "Items");
        _Collection.ItemTemplate = cardTemplate;

        _Refresh.Content = _Collection;
        _Refresh.Command = new Command(Fetch);

        _ContentLayout.Children.Add(_Search.Row(0));
        _ContentLayout.Children.Add(_Refresh.Row(1));
        _ContentLayout.Children.Add(_Pagination.Row(2).Center());

        if (isEditable)
        {
            _ContentLayout.Children.Add(_Add.Row(0).RowSpan(3).End().Bottom().ZIndex(1));
        }

        Content = _ContentLayout;

        _Search.TextChanged += SearchChanged;
        _Pagination.PageChanged += PageChanged;
        _Add.Clicked += AddClicked;
    }
    ~MaterialList()
    {
        _Search.TextChanged -= SearchChanged;
        _Pagination.PageChanged -= PageChanged;
        _Add.Clicked -= AddClicked;
    }
    #endregion

    #region Helpers
    public void FetchPublic() => _Refresh.IsRefreshing = true;

    private async void Fetch()
    {
        await _ViewModel.GetItems();

        _ContentLayout.Children.Remove(_NoItemsUI);
        if (_ViewModel.Items.Count <= 0)
        {
            _ContentLayout.Children.Add(_NoItemsUI.Row(1));
        }

        _Refresh.IsRefreshing = false;
        FetchedNewPage?.Invoke(null, null);
    }

    private void SearchChanged(object sender, TextChangedEventArgs e)
    {
        _SearchDebouncer.Debounce(() =>
        {
            Fetch();
        });
    }

    private void PageChanged(object sender, PageChangedEventArgs e)
    {
        Fetch();
    }

    private void AddClicked(object sender, ClickedEventArgs e)
    {
        AddItemClicked?.Invoke(sender, e);
    }
    #endregion
}
