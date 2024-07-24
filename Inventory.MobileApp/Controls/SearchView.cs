using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.Utilities;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Controls;

public interface IFilter
{
    public string Placeholder { get; set; }
    public string Text { get; set; }
    public Action? Clicked { get; set; }
}

public interface ISearchViewModel<T>
{
    public List<IFilter> Filters { get; set; }
    public List<T> Items { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public int Page { get; set; }
    public Task Search(string search);
}

public class SearchView<T> : ContentView
{
    public event EventHandler? AddItem;

    public static readonly BindableProperty CardTemplateProperty = BindableProperty.Create(nameof(CardTemplate), typeof(DataTemplate), typeof(SearchView<T>), null);
    public DataTemplate CardTemplate { get => (DataTemplate)GetValue(CardTemplateProperty); set => SetValue(CardTemplateProperty, value); }

    public static readonly BindableProperty SearchLayoutProperty = BindableProperty.Create(nameof(SearchLayout), typeof(IItemsLayout), typeof(SearchView<T>), null);
    public IItemsLayout SearchLayout { get => (IItemsLayout)GetValue(SearchLayoutProperty); set => SetValue(SearchLayoutProperty, value); }

    public static readonly BindableProperty CanAddItemsProperty = BindableProperty.Create(nameof(CanAddItems), typeof(bool), typeof(SearchView<T>), true);
    public bool CanAddItems { get => (bool)GetValue(CanAddItemsProperty); set => SetValue(CanAddItemsProperty, value); }

    public static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(SearchView<T>), false);
    public bool IsLoading { get => (bool)GetValue(IsLoadingProperty); set => SetValue(IsLoadingProperty, value); }

    public static readonly BindableProperty ShowSearchProperty = BindableProperty.Create(nameof(ShowSearch), typeof(bool), typeof(SearchView<T>), true);
    public bool ShowSearch { get => (bool)GetValue(ShowSearchProperty); set => SetValue(ShowSearchProperty, value); }

    public static readonly BindableProperty FiltersProperty = BindableProperty.Create(nameof(Filters), typeof(List<IFilter>), typeof(SearchView<T>));
    public List<IFilter> Filters { get => (List<IFilter>)GetValue(FiltersProperty); set => SetValue(FiltersProperty, value); }

    private readonly Debouncer _SearchDebounce = new();
    private readonly ISearchViewModel<T> _SearchVM;
    private readonly MaterialEntry _SearchEntry = new()
    {
        EntryStyle = EntryStyle.Search,
    };
    private readonly RefreshView _Refresh = new RefreshView();
    private readonly CollectionView _SearchItems = new CollectionView();
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Auto, Star, 24),
        RowSpacing = 8
    };
    private readonly Button _AddButton = new()
    {
        CornerRadius = 30,
        Padding = 0,
        HeightRequest = 60,
        WidthRequest = 60,
        ZIndex = 999
    };
    private readonly Button _NextButton = new()
    {
        Padding = 0,
        Margin = 0,
        HeightRequest = 24,
        MaximumHeightRequest = 24,
        MinimumHeightRequest = 24,
        CornerRadius = 12,
    };
    private readonly Button _PreviousButton = new()
    {
        Padding = 0,
        Margin = 0,
        HeightRequest = 24,
        MaximumHeightRequest = 24,
        MinimumHeightRequest = 24,
        CornerRadius = 12,
    };
    private readonly Label _PageLabel = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center
    };
    private readonly Label _NoItemsLabel = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        Text = LanguageService.Instance["No items found"]
    };
    private readonly Border _CountBacking = new Border().BackgroundColor(UIService.Color("Primary")).Padding(0).BorderShape(new RoundRectangle { CornerRadius = 15 }).Height(30).MinWidth(30);
    private readonly Label _CountLabel = new Label().FontSize(16).Center().TextColor(Colors.White);
    private readonly ActivityIndicator _Loading = new() { WidthRequest = 25, IsVisible = false };
    private readonly Image _Filter = new Image().ApplyMaterialIcon(MaterialIcon.Filter_alt, 24, UIService.Color("Primary"));
    private Grid? _NoFilterLayout;
    private Grid? _FilterLayout;

    public SearchView(ISearchViewModel<T> searchVM)
    {
        BindingContext = searchVM;
        this.SetBinding(FiltersProperty, "Filters");

        Padding = new Thickness(8, 12, 8, 12);

        _CountBacking.Content = new Grid { Children = { _CountLabel } };

        _PreviousButton.ApplyMaterialIcon(MaterialIcon.Chevron_left, 18, Colors.White);
        _NextButton.ApplyMaterialIcon(MaterialIcon.Chevron_right, 18, Colors.White);
        _PreviousButton.Clicked += Previous;
        _NextButton.Clicked += Next;
        _AddButton.ApplyMaterialIcon(MaterialIcon.Add, 21, Colors.White);
        _AddButton.Clicked += (s, e) => AddItem?.Invoke(this, EventArgs.Empty);

        _SearchVM = searchVM;

        _Refresh.Command = new Command(Search);
        _Refresh.Content = _SearchItems;

        _ContentLayout.Children.Add(_AddButton.Row(0).RowSpan(3).Bottom().End());
        _ContentLayout.Children.Add(_Refresh.Row(1));
        _ContentLayout.Children.Add(new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                _PreviousButton,
                _PageLabel,
                _NextButton,
            }
        }.Row(2).Center());

        Content = _ContentLayout;

        _SearchEntry.TextChanged += SearchTextChanged;

        LayoutSearch();
    }

    private void LayoutSearch()
    {
        _NoFilterLayout?.Clear();
        _FilterLayout?.Clear();

        _FilterLayout = new Grid
        {
            ColumnSpacing = 8,
            ColumnDefinitions = Columns.Define(Auto, Star, Auto),
            Children =
            {
                _CountBacking,
                _SearchEntry.Column(1),
                new HorizontalStackLayout
                {
                    Spacing = 8,
                    Children =
                    {
                        _Filter,
                        _Loading
                    }
                }.Column(2)
            }
        };
        _NoFilterLayout = new Grid
        {
            ColumnSpacing = 8,
            ColumnDefinitions = Columns.Define(Auto, Star, 32),
            Children =
            {
                _CountBacking,
                _SearchEntry.Column(1).ColumnSpan(2),
                _Loading.Column(2)
            }
        };

        _ContentLayout.Remove(_FilterLayout);
        _ContentLayout.Remove(_NoFilterLayout);

        if (Filters.Count > 0)
        {
            _ContentLayout.Children.Add(_FilterLayout.Row(0));
        }
        else
        {
            _ContentLayout.Children.Add(_NoFilterLayout.Row(0));
        }
    }

    public void TriggerRefresh() => _Refresh.IsRefreshing = true;
    private async void Search()
    {
        _Refresh.IsRefreshing = false; // we have a secondary loading indicator, so we don't need pull to refresh one

        IsLoading = true;

        _ContentLayout.Children.Remove(_NoItemsLabel);
        _ContentLayout.Children.Add(_NoItemsLabel.Row(1).Center().ZIndex(10));

        _SearchItems.ItemsSource = null;
        await _SearchVM.Search(_SearchEntry.Text);
        
        if (_SearchVM.Items.Count > 0)
            _ContentLayout.Children.Remove(_NoItemsLabel);

        _SearchItems.ItemsSource = _SearchVM.Items;
        _PageLabel.Text = $"{_SearchVM.Page + 1} / {_SearchVM.TotalPages}";
        _CountLabel.Text = $"{_SearchVM.Total}";

        IsLoading = false;
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == SearchLayoutProperty.PropertyName && SearchLayout != null)
        {
            _SearchItems.ItemsLayout = SearchLayout;
        }
        else if (propertyName == CardTemplateProperty.PropertyName && CardTemplate != null)
        {
            _SearchItems.ItemTemplate = CardTemplate;
        }
        else if (propertyName == CanAddItemsProperty.PropertyName)
        {
            _ContentLayout.Children.Remove(_AddButton);
            if (CanAddItems)
            {
                _ContentLayout.Children.Add(_AddButton.Row(0).RowSpan(3).Bottom().End());
            }
        }
        else if (propertyName == IsLoadingProperty.PropertyName)
        {
            if (IsLoading)
            {
                _SearchEntry.ColumnSpan(1);
                _Loading.IsVisible = true;
                _Loading.IsRunning = true;
                _Loading.IsEnabled = true;
            }
            else
            {
                _SearchEntry.ColumnSpan(Filters.Count > 0 ? 1 : 2);
                _Loading.IsVisible = false;
                _Loading.IsRunning = false;
                _Loading.IsEnabled = false;
            }
        }
        else if (propertyName == ShowSearchProperty.PropertyName)
        {
            _SearchEntry.IsVisible = ShowSearch;
        }
        else if (propertyName == FiltersProperty.PropertyName)
        {
            LayoutSearch();
        }
    }

    private void Next(object? sender, EventArgs e)
    {
        int actualPage = _SearchVM.Page + 1;
        if (actualPage + 1 > _SearchVM.TotalPages)
        {
            return;
        }

        _SearchVM.Page++;
        TriggerRefresh();
    }

    private void Previous(object? sender, EventArgs e)
    {
        if (_SearchVM.Page - 1 < 0)
        {
            return;
        }

        _SearchVM.Page--;
        TriggerRefresh();
    }

    private void SearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _SearchDebounce.Debounce(TriggerRefresh);
    }
}
