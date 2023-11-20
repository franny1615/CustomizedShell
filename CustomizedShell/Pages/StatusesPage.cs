using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using CustomizedShell.Models;
using CustomizedShell.Services;
using CustomizedShell.ViewModels;
using CustomizedShell.Views;
using Maui.Components.Controls;
using Maui.Components.Utilities;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace CustomizedShell.Pages;

public class StatusesPage : BasePage
{
    #region Private Properties
    private readonly Debouncer _SearchDebouncer = new();
    private readonly Color _Primary = Application.Current.Resources["Primary"] as Color;
    private readonly Color _CardLight = Application.Current.Resources["CardColorLight"] as Color;
    private readonly Color _CardDark = Application.Current.Resources["CardColorDark"] as Color;
    private DataViewModel _DataViewModel => (DataViewModel) BindingContext;
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(Auto, Star),
        RowSpacing = 16,
        Padding = 16
    };
    private readonly SearchBarView _Search = new()
    {
        SearchImageSource = "search.png",
        ClearImageSource = "close.png",
        Placeholder = LanguageService.Instance["Search"]
    };
    private readonly CollectionView _StatusCollection = new()
    {
        ZIndex = 0,
        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 16 },
    };
    private readonly FloatingActionButton _AddStatus = new()
    {
        ZIndex = 1,
        ImageSource = "add.png",
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        FABStyle = FloatingActionButtonStyle.Regular
    };
    #endregion

    #region Constructor
    public StatusesPage(DataViewModel dataViewModel)
    {
        OverrideBackButtonText();

        BindingContext = dataViewModel;

        Title = Lang["Statuses"];

        _Search.SetAppThemeColor(SearchBarView.SearchBackgroundColorProperty, _CardLight, _CardDark);
        _Search.SetAppThemeColor(SearchBarView.ContentColorProperty, Colors.Black, Colors.White);

        _ContentLayout.Children.Add(_Search.Row(0));
        _ContentLayout.Children.Add(_StatusCollection.Row(1));
        _ContentLayout.Children.Add(_AddStatus.Row(1).End().Bottom());

        _StatusCollection.SetBinding(CollectionView.ItemsSourceProperty, "Statuses");
        _StatusCollection.ItemTemplate = new DataTemplate(() => 
        {
            var view = new MiniCardView();
            view.Clicked += EditStatus;
            view.BindingContext = new Binding(".");
            view.SetBinding(MiniCardView.TitleProperty, "Name");
            view.SetAppThemeColor(MiniCardView.CardColorProperty, _CardLight, _CardDark);
            view.ImageBackgroundColor = _Primary;
            view.ImageSource = "done.png";

            return view;
        });

        Content = _ContentLayout;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _DataViewModel.GetAllStatuses(_Search.Text);
        _AddStatus.Clicked += AddStatus;
        _Search.SearchChanged += SearchChanged;
    }

    protected override void OnDisappearing()
    {
        _AddStatus.Clicked -= AddStatus;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private void AddStatus(object sender, EventArgs e)
    {
        var popup = new StatusDetailsPopupView(
            new Status(), 
            StatusDetailsMode.New, 
            _DataViewModel);

        popup.Closed += async (_, _) => { await _DataViewModel.GetAllStatuses(_Search.Text); };

        this.ShowPopup(popup); 
    }

    private void EditStatus(object sender, EventArgs e)
    {
        if (sender is MiniCardView view &&
            view.BindingContext is Status status)
        {
            var popup = new StatusDetailsPopupView(
                status, 
                StatusDetailsMode.Edit, 
                _DataViewModel);
            
            popup.Closed += async (_, _) => { await _DataViewModel.GetAllStatuses(_Search.Text); };

            this.ShowPopup(popup); 
        }
    }

    private void SearchChanged(object sender, TextChangedEventArgs e)
    {
        _SearchDebouncer.Debounce(async () =>
        {
            await _DataViewModel.GetAllStatuses(_Search.Text);
        });
    }
    #endregion
}
