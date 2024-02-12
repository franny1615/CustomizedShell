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
    #region Private Properties
    private IMaterialListVM<T> _ViewModel => (IMaterialListVM<T>) BindingContext;
    private readonly Debouncer _SearchDebouncer = new(0.5);
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Auto, Star, Auto),
        RowSpacing = 8,
        Padding = 16
    };
    private readonly CollectionView _Collection = new()
    {
        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 8 },
        ZIndex = 0,
    };
    private bool _IsLoading = false;
    private readonly ProgressBar _BusyIndicator = new() { ZIndex = 1, WidthRequest = 200 };
    private readonly MaterialEntry _Search;
    private readonly MaterialPagination _Pagination;
    private readonly View _NoItemsUI;
    #endregion

    #region Constructor
    public MaterialList(
        View emptyListUI,
        DataTemplate cardTemplate,
        IMaterialListVM<T> viewModel)
    {
        _NoItemsUI = emptyListUI;

        BindingContext = viewModel;

        _Search = new(viewModel.SearchModel);
        _Pagination = new(viewModel.PaginationModel);

        _Pagination.ContentColor = Colors.White;
        _Pagination.BackgroundColor = Application.Current.Resources["Primary"] as Color;

        _Collection.SetBinding(CollectionView.ItemsSourceProperty, "Items");
        _Collection.ItemTemplate = cardTemplate;

        _ContentLayout.Children.Add(_Search.Row(0));
        _ContentLayout.Children.Add(_Collection.Row(1));
        _ContentLayout.Children.Add(_Pagination.Row(2).Center());

        Content = _ContentLayout;

        _Search.TextChanged += SearchChanged;
        _Pagination.PageChanged += PageChanged;
    }
    ~MaterialList()
    {
        _Search.TextChanged -= SearchChanged;
        _Pagination.PageChanged -= PageChanged;
    }
    #endregion

    #region Helpers
    public async void Fetch()
    {
        StartBusyIndicator();
        await _ViewModel.GetItems();

        _ContentLayout.Children.Remove(_NoItemsUI);
        if (_ViewModel.Items.Count <= 0)
        {
            _ContentLayout.Children.Add(_NoItemsUI.Row(1));
        }

        EndBusyIndicator();
    }

    private void StartBusyIndicator()
    {
        _IsLoading = true;
        _ContentLayout.Children.Add(_BusyIndicator.Top().CenterHorizontal().Row(1));
        _BusyIndicator.ProgressColor = Application.Current.Resources["Secondary"] as Color;
        _BusyIndicator.Progress = 1;
        Task.Run(async () =>
        {
            while (_IsLoading)
            {
                await _BusyIndicator.FadeTo(0.25);
                await _BusyIndicator.FadeTo(1);
            }
        });
    }

    private void EndBusyIndicator()
    {
        _ContentLayout.Children.Remove(_BusyIndicator);
        _IsLoading = false;
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
    #endregion
}
