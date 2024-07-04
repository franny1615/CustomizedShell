using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class InventorySearchPage : BasePage
{
    private readonly InventorySearchViewModel _ViewModel;
    private readonly SearchView<Models.Inventory> _Search;

    public InventorySearchPage(InventorySearchViewModel invSearchVM)
    {
        _ViewModel = invSearchVM;
        _Search = new(invSearchVM);
        _Search.SearchLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 12 };
        _Search.CardTemplate = new DataTemplate(() =>
        {
            var view = new InventoryCardView();
            view.SetBinding(BindingContextProperty, ".");
            view.SetBinding(InventoryCardView.DescriptionProperty, "Description");
            view.SetBinding(InventoryCardView.StatusProperty, "Status");
            view.SetBinding(InventoryCardView.QuantityProperty, "Quantity");
            view.SetBinding(InventoryCardView.QuantityTypeProperty, "QuantityType");
            view.SetBinding(InventoryCardView.BarcodeProperty, "Barcode");
            view.SetBinding(InventoryCardView.LocationProperty, "Location");
            view.SetBinding(InventoryCardView.LastEditedOnProperty, "LastEditedOn");
            view.SetBinding(InventoryCardView.CreatedOnProperty, "CreatedOn");
            view.EditDescription += EditDescription;
            view.EditLocation += EditLocation;
            view.EditQuantity += EditQuantity;
            view.EditQuantityType += EditQuantityType;
            view.EditStatus += EditStatus;
            view.KebabMenu += MoreOptions;

            return view;
        });
        _Search.AddItem += AddInventory;

        Title = LanguageService.Instance["Inventory"];
        Content = _Search;
    }

    private async void MoreOptions(object? sender, EventArgs e)
    {
        if (sender is InventoryCardView card && card.BindingContext is Models.Inventory inventory)
        {
            string delete = LanguageService.Instance["Delete"];
            string print = LanguageService.Instance["Print"];
            string choice = await DisplayActionSheet(
                    LanguageService.Instance["Options"],
                    LanguageService.Instance["Cancel"],
                    null,
                    [
                        delete,
                    print,
                    ]);

            if (choice == delete)
            {
                Delete(inventory);
            }
            else if (choice == print)
            {
                string path = Path.Combine(FileSystem.CacheDirectory, "barcode.png");
                File.WriteAllBytes(path, card.CurrentBarcode);
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = LanguageService.Instance["Share Barcode"],
                    File = new ShareFile(path)
                });
            }
        }
    }

    private async void Delete(Models.Inventory inventory)
    {
        _Search.IsLoading = true;

        var response = await _ViewModel.DeleteInventory(inventory.Id);
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
            case DeleteResult.LinkedToOtherItems: // shouldn't happen but in here for completeness.
                await DisplayAlert(
                    LanguageService.Instance["In Use"],
                    $"{inventory.Description}.",
                    LanguageService.Instance["OK"]);
                break;
        }

        _Search.IsLoading = false; // fail safe
    }

    private void EditStatus(object? sender, EventArgs e)
    {
        
    }

    private void EditQuantityType(object? sender, EventArgs e)
    {
        
    }

    private void EditQuantity(object? sender, EventArgs e)
    {
        
    }

    private void EditLocation(object? sender, EventArgs e)
    {
        
    }

    private async void EditDescription(object? sender, EventArgs e)
    {
        if (sender is InventoryCardView card && card.BindingContext is Models.Inventory inventory)
        {
            string input = await DisplayPromptAsync(
                LanguageService.Instance["Edit Description"],
                LanguageService.Instance["Enter new description below."],
                LanguageService.Instance["OK"],
                LanguageService.Instance["Cancel"]);

            if (string.IsNullOrEmpty(input))
                return;

            string cacheInventory = inventory.Description;
            inventory.Description = input;

            _Search.IsLoading = true;
            var response = await _ViewModel.UpdateInventory(inventory);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                _Search.IsLoading = false;
                this.DisplayCommonError(response.ErrorMessage);
                return;
            }

            if (!response.Data)
            {
                inventory.Description = cacheInventory;
            }

            _Search.IsLoading = false;
        }
    }

    private void AddInventory(object? sender, EventArgs e)
    {
        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.TriggerRefresh();
    }
}
