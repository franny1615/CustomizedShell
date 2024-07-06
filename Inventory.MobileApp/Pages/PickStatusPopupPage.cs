using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Pages.Components;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Pages;

public class PickStatusPopupPage : PopupPage
{
    private readonly StatusSearchViewModel _StatusSearchVM;
    private readonly SearchView<Status> _Search;
    private readonly Action<Status> _PickedStatus;
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

    public PickStatusPopupPage(
        StatusSearchViewModel statusVM,
        string title,
        Action<Status> pickedStatus,
        Action closed)
    {
        _StatusSearchVM = statusVM;
        _PickedStatus = pickedStatus;
        _Closed = closed;
        _Title.Text = title;
        _Search = new SearchView<Status>(statusVM);
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CanAddItems = false;
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new StatusCardView();
            view.SetBinding(BindingContextProperty, ".");
            view.SetBinding(StatusCardView.DescriptionProperty, "Description");
            view.IsSelectable = true;
            view.Selected += SelectedStatus;

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

    private async void SelectedStatus(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
        if (sender is StatusCardView card && card.BindingContext is Status status)
        {
            _PickedStatus?.Invoke(status);
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
