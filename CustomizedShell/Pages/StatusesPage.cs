using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using CustomizedShell.Views;
using Maui.Components.Controls;

namespace CustomizedShell.Pages;

public class StatusesPage : BasePage
{
    #region Private Properties
    private readonly Color _Primary = Application.Current.Resources["Primary"] as Color;
    private readonly Color _CardLight = Application.Current.Resources["CardColorLight"] as Color;
    private readonly Color _CardDark = Application.Current.Resources["CardColorDark"] as Color;
    private DataViewModel _DataViewModel => (DataViewModel) BindingContext;
    private readonly Grid _ContentLayout = new()
    {
        Padding = 16
    };
    private readonly CollectionView _StatusCollection = new()
    {
        ZIndex = 0,
        ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 8 },
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

        _ContentLayout.Children.Add(_StatusCollection);
        _ContentLayout.Children.Add(_AddStatus.End().Bottom());

        _StatusCollection.SetBinding(CollectionView.ItemsSourceProperty, "Statuses");
        _StatusCollection.ItemTemplate = new DataTemplate(() => 
        {
            var view = new CardView();
            view.Clicked += EditStatus;
            view.BindingContext = new Binding(".");
            view.SetBinding(CardView.TitleProperty, "Name");
            view.SetAppThemeColor(CardView.CardColorProperty, _CardLight, _CardDark);
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
        _ = _DataViewModel.GetAllStatuses();
        _AddStatus.Clicked += AddStatus;
    }

    protected override void OnDisappearing()
    {
        _DataViewModel.SearchText = string.Empty;
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

        popup.Closed += async (_, _) => { await _DataViewModel.GetAllStatuses(); };

        this.ShowPopup(popup); 
    }

    private void EditStatus(object sender, EventArgs e)
    {
        if (sender is CardView view &&
            view.BindingContext is Status status)
        {
            var popup = new StatusDetailsPopupView(
                status, 
                StatusDetailsMode.Edit, 
                _DataViewModel);
            
            popup.Closed += async (_, _) => { await _DataViewModel.GetAllStatuses(); };

            this.ShowPopup(popup); 
        }
    }
    #endregion
}
