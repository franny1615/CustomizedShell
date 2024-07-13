using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Pages.Components;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class InventorySearchPage : BasePage
{
    private readonly InventorySearchViewModel _ViewModel;
    private readonly SearchView<Models.Inventory> _Search;
    private Random RAND = new Random();
    private bool _IsEditing = false;

    public InventorySearchPage(InventorySearchViewModel invSearchVM)
    {
        int permissions = SessionService.CurrentPermissions.InventoryPermissions;
        int canAddPerm = (int)InventoryPermissions.CanAddInventory;
        int canAddPermInt = permissions & canAddPerm;
        bool canAdd = canAddPermInt == canAddPerm;

        _ViewModel = invSearchVM;
        _Search = new(invSearchVM);
        _Search.CanAddItems = canAdd;
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

            List<string> options = new List<string>();
            if (PermsUtils.IsAllowed(InventoryPermissions.CanDeleteInv))
                options.Add(delete);
            options.Add(print);

            string choice = await DisplayActionSheet(
                    LanguageService.Instance["Options"],
                    LanguageService.Instance["Cancel"],
                    null,
                    options.ToArray());

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
        if (sender is InventoryCardView card && card.BindingContext is Models.Inventory inventory)
        {
            _IsEditing = true;
            Navigation.PushModalAsync(PageService.PickStatus(
                LanguageService.Instance["Pick Status"],
                picked: async (status) =>
                {
                    if (status == null)
                        return;

                    string descCache = inventory.QuantityType;
                    int idCache = inventory.QtyTypeID;

                    inventory.StatusID = status.Id;
                    inventory.Status = status.Description;

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
                        inventory.StatusID = idCache;
                        inventory.Status = descCache;
                    }

                    _Search.IsLoading = false;
                    _IsEditing = false;

                    // TODO: keep track of this issue, once resolved we can end the force refresh
                    // https://github.com/dotnet/maui/issues/14363
                    // basically on iOS, the card view height doesn't update even through the inner content has been resized.
                    _Search.TriggerRefresh();
                },
                canceled: () =>
                {
                    _IsEditing = false;
                }));
        }
    }

    private void EditQuantityType(object? sender, EventArgs e)
    {
        if (sender is InventoryCardView card && card.BindingContext is Models.Inventory inventory)
        {
            _IsEditing = true;
            Navigation.PushModalAsync(PageService.PickQtyType(
                LanguageService.Instance["Pick Quantity Type"],
                picked: async (qtyType) =>
                {
                    if (qtyType == null)
                        return;

                    string descCache = inventory.QuantityType;
                    int idCache = inventory.QtyTypeID;

                    inventory.QtyTypeID = qtyType.Id;
                    inventory.QuantityType = qtyType.Description;

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
                        inventory.QtyTypeID = idCache;
                        inventory.QuantityType = descCache;
                    }

                    _Search.IsLoading = false;
                    _IsEditing = false;

                    // TODO: keep track of this issue, once resolved we can end the force refresh
                    // https://github.com/dotnet/maui/issues/14363
                    // basically on iOS, the card view height doesn't update even through the inner content has been resized.
                    _Search.TriggerRefresh();
                },
                canceled: () =>
                {
                    _IsEditing = false;
                }));
        }
    }

    private void EditQuantity(object? sender, EventArgs e)
    {
        if (sender is InventoryCardView card && card.BindingContext is Models.Inventory inventory)
        {
            _IsEditing = true;
            Navigation.PushModalAsync(PageService.TakeUserInput(
                LanguageService.Instance["Edit Quantity"],
                inventory.Quantity.ToString(),
                Keyboard.Numeric,
                submitted: async (text) =>
                {
                    if (string.IsNullOrEmpty(text))
                        return;

                    var isNumber = int.TryParse(text, out int newQty);
                    if (!isNumber)
                        return;

                    int cache = inventory.Quantity;
                    inventory.Quantity = newQty;

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
                        inventory.Quantity = cache;
                    }

                    _Search.IsLoading = false;
                    _IsEditing = false;

                    // TODO: keep track of this issue, once resolved we can end the force refresh
                    // https://github.com/dotnet/maui/issues/14363
                    // basically on iOS, the card view height doesn't update even through the inner content has been resized.
                    _Search.TriggerRefresh();
                },
                canceled: () =>
                {
                    _IsEditing = false;
                }));
        }
    }

    private void EditLocation(object? sender, EventArgs e)
    {
        if (sender is InventoryCardView card && card.BindingContext is Models.Inventory inventory)
        {
            _IsEditing = true;
            Navigation.PushModalAsync(PageService.PickLocation(
                LanguageService.Instance["Pick Location"],
                picked: async (location) =>
                {
                    if (location == null) 
                        return;

                    string locationDescCache = inventory.Location;
                    int locationIDCache = inventory.LocationID;

                    inventory.LocationID = location.Id;
                    inventory.Location = location.Description;

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
                        inventory.LocationID = locationIDCache;
                        inventory.Location = locationDescCache;
                    }

                    _Search.IsLoading = false;
                    _IsEditing = false;

                    // TODO: keep track of this issue, once resolved we can end the force refresh
                    // https://github.com/dotnet/maui/issues/14363
                    // basically on iOS, the card view height doesn't update even through the inner content has been resized.
                    _Search.TriggerRefresh();
                },
                canceled: () =>
                {
                    _IsEditing = false;
                }));
        }
    }

    private void EditDescription(object? sender, EventArgs e)
    {
        if (sender is InventoryCardView card && card.BindingContext is Models.Inventory inventory)
        {
            _IsEditing = true;
            Navigation.PushModalAsync(new UserInputPopupPage(
                LanguageService.Instance["Edit Description"],
                inventory.Description,
                Keyboard.Plain,
                submitted: async (text) =>
                {
                    if (string.IsNullOrEmpty(text))
                        return;

                    string cacheInventory = inventory.Description;
                    inventory.Description = text;

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
                    _IsEditing = false;

                    // TODO: keep track of this issue, once resolved we can end the force refresh
                    // https://github.com/dotnet/maui/issues/14363
                    // basically on iOS, the card view height doesn't update even through the inner content has been resized.
                    _Search.TriggerRefresh();
                },
                canceled: () =>
                {
                    _IsEditing = false;
                }));
        }
    }

    private void AddInventory(object? sender, EventArgs e)
    {
        _IsEditing = true;
        Navigation.PushAsync(PageService.AddInventory(
            LanguageService.Instance["Add Inventory"],
            new Models.Inventory 
            { 
                Description = string.Empty,
                Quantity = 0,
                Barcode = $"{RAND.Next(1000000, 9999999)}",
                LastEditedOn = DateTime.Now, 
                CreatedOn = DateTime.Now 
            },
            add: async (inventory) =>
            {
                if (inventory == null) 
                    return;

                _Search.IsLoading = true;
                var response = await _ViewModel.InsertInventory(inventory);
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    _Search.IsLoading = false;
                    this.DisplayCommonError(response.ErrorMessage);
                    return;
                }

                _Search.IsLoading = false;
                _IsEditing = false;

                _Search.TriggerRefresh();
            }));
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
