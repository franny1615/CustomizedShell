using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class QuantityTypesSearchPage : BasePage
{
    private readonly QuantityTypesSearchViewModel _ViewModel;
    private readonly SearchView<QuantityType> _Search;
    private bool _IsEditing = false;

    public QuantityTypesSearchPage(QuantityTypesSearchViewModel viewModel)
    {
        _ViewModel = viewModel;
        _Search = new(_ViewModel);
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new QuantityTypeCardView();
            view.SetBinding(BindingContextProperty, ".");
            view.SetBinding(QuantityTypeCardView.DescriptionProperty, "Description");
            view.Delete += DeleteQtyType;
            view.Edit += UpdateQtyType;

            return view;
        });
        _Search.AddItem += AddQtyType;

        Title = LanguageService.Instance["Quantity Types"];
        Content = _Search;
    }

    private void AddQtyType(object? sender, EventArgs e)
    {
        _IsEditing = true;
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Enter the quantity type below."],
            "",
            Keyboard.Plain,
            submitted: async (qtyType) =>
            {
                if (string.IsNullOrEmpty(qtyType)) // canceled or entered empty text, don't do anything.
                    return;

                _Search.IsLoading = true;

                var response = await _ViewModel.InsertQtyType(qtyType);
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    _Search.IsLoading = false;
                    this.DisplayCommonError(response.ErrorMessage);
                    return;
                }

                _Search.TriggerRefresh();
                _Search.IsLoading = false; // fail safe
                
                _IsEditing = false;
            },
            canceled: () =>
            {
                _IsEditing = false;
            }));
    }

    private async void DeleteQtyType(object? sender, EventArgs e)
    {
        if (sender is QuantityTypeCardView card && card.BindingContext is QuantityType qtyType)
        {
            _Search.IsLoading = true;

            var response = await _ViewModel.DeleteQtyType(qtyType.Id);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                _Search.IsLoading = false;
                this.DisplayCommonError(response.ErrorMessage);
                return;
            }

            switch (response.Data)
            {
                case DeleteResult.SuccesfullyDeleted:
                    _Search.TriggerRefresh();
                    break;
                case DeleteResult.LinkedToOtherItems:
                    await DisplayAlert(
                        LanguageService.Instance["In Use"],
                        $"{qtyType.Description} - {LanguageService.Instance["is in use in at least one inventory item"]}.",
                        LanguageService.Instance["OK"]);
                    break;
            }

            _Search.IsLoading = false; // fail safe
        }
    }

    private void UpdateQtyType(object? sender, EventArgs e)
    {
        if (sender is QuantityTypeCardView card && card.BindingContext is QuantityType qtyType)
        {
            _IsEditing = true;
            Navigation.PushModalAsync(PageService.TakeUserInput(
                LanguageService.Instance["Enter the quantity type below."],
                qtyType.Description,
                Keyboard.Plain,
                submitted: async (qtyTypeStr) =>
                {
                    if (string.IsNullOrEmpty(qtyTypeStr)) // canceled or entered empty text, don't do anything.
                        return;

                    _Search.IsLoading = true;

                    qtyType.Description = qtyTypeStr;
                    var response = await _ViewModel.UpdateQtyType(qtyType);

                    if (!string.IsNullOrEmpty(response.ErrorMessage))
                    {
                        _Search.IsLoading = false;
                        this.DisplayCommonError(response.ErrorMessage);
                        return;
                    }

                    _Search.TriggerRefresh();
                    _Search.IsLoading = false; // fail safe

                    _IsEditing = false;
                },
                canceled: () =>
                {
                    _IsEditing = false;
                }));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_IsEditing)
        {
            _Search.TriggerRefresh();
        }
    }
}
