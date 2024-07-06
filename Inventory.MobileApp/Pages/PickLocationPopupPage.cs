using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Pages.Components;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using Location = Inventory.MobileApp.Models.Location;

namespace Inventory.MobileApp.Pages;

public class PickLocationPopupPage : PopupPage
{
    private readonly LocationSearchViewModel _LocationSearchVM;
    private readonly SearchView<Location> _Search;
    private readonly Action<Location> _PickedLocation;
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

    public PickLocationPopupPage(
        LocationSearchViewModel locationVM,
        string title,
        Action<Location> pickedLocation,
        Action closed)
    {
        _LocationSearchVM = locationVM;
        _PickedLocation = pickedLocation;
        _Closed = closed;
        _Title.Text = title;
        _Search = new SearchView<Location>(locationVM);
        _Search.CanAddItems = false;
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new LocationCardView();
            view.SetBinding(BindingContextProperty, ".");
            view.SetBinding(LocationCardView.DescriptionProperty, "Description");
            view.SetBinding(LocationCardView.BarcodeProperty, "Barcode");
            view.IsSelectable = true;
            view.Selected += SelectedLocation;

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

    private async void SelectedLocation(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
        if (sender is LocationCardView card && card.BindingContext is Location loc)
        {
            _PickedLocation?.Invoke(loc);
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
