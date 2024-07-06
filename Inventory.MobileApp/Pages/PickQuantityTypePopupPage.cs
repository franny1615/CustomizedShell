using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Pages.Components;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Pages;

public class PickQuantityTypePopupPage : PopupPage
{
    private readonly QuantityTypesSearchViewModel _QtyTypeVM;
    private readonly SearchView<QuantityType> _Search;
    private readonly Action<QuantityType> _PickedQtyType;
    private readonly Action _Closed;
    private readonly Label _Title = new Label().FontSize(16).Bold().Center();
    private readonly Button _Dismiss = new Button
    {
        Text = LanguageService.Instance["Cancel"],
        BackgroundColor = Color.FromArgb("#646464"),
        TextColor = Colors.White,
    };
    private readonly Grid _ContentLayout = new Grid
    {
        RowDefinitions = Rows.Define(Auto, Star, Auto),
        RowSpacing = 8,
        Padding = 8
    };

    public PickQuantityTypePopupPage(
        QuantityTypesSearchViewModel qtyTypeVM,
        string title,
        Action<QuantityType> pickedQtyType,
        Action closed)
    {
        _QtyTypeVM = qtyTypeVM;
        _PickedQtyType = pickedQtyType;
        _Closed = closed;
        _Title.Text = title;
        _Search = new SearchView<QuantityType>(qtyTypeVM);
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CanAddItems = false;
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new QuantityTypeCardView();
            view.SetBinding(BindingContextProperty, ".");
            view.SetBinding(QuantityTypeCardView.DescriptionProperty, "Description");
            view.IsSelectable = true;
            view.Selected += SelectedQtyType;

            return view;
        });

        _ContentLayout.SetDynamicResource(BackgroundProperty, "PageColor");
        _ContentLayout.Add(_Title.Row(0));
        _ContentLayout.Add(_Search.Row(1));
        _ContentLayout.Add(_Dismiss.Row(2));

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;

        _Dismiss.Clicked += Close;
    }

    private async void SelectedQtyType(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
        if (sender is QuantityTypeCardView card && card.BindingContext is QuantityType qtyType) 
        {
            _PickedQtyType?.Invoke(qtyType);
        }
    }

    private void Close(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
        _Closed?.Invoke();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.TriggerRefresh();
#if ANDROID
        Platform.CurrentActivity?.Window?.SetSoftInputMode(Android.Views.SoftInput.AdjustNothing);
#endif
    }
}
