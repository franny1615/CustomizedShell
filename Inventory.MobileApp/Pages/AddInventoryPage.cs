using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Services;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Pages;

public class AddInventoryPage : BasePage
{
    private readonly Models.Inventory _Inventory;
    private readonly Action<Models.Inventory> _Add;
    private readonly InventoryCardView _EditableCard = new ();
    private readonly Grid _ContentLayout = new Grid
    {
        RowDefinitions = Rows.Define(Star, Auto),
        RowSpacing = 12,
        Padding = 8
    };
    private readonly Button _AddButton = new Button
    { 
        Text = LanguageService.Instance["Add"]
    };

    public AddInventoryPage(
        string title,
        Models.Inventory inventory,
        Action<Models.Inventory> add)
    {
        Title = title;
        _Inventory = inventory;
        _Add = add;

        _EditableCard.BindingContext = _Inventory;
        _EditableCard.SetBinding(InventoryCardView.DescriptionProperty, "Description");
        _EditableCard.SetBinding(InventoryCardView.StatusProperty, "Status");
        _EditableCard.SetBinding(InventoryCardView.QuantityProperty, "Quantity");
        _EditableCard.SetBinding(InventoryCardView.QuantityTypeProperty, "QuantityType");
        _EditableCard.SetBinding(InventoryCardView.BarcodeProperty, "Barcode");
        _EditableCard.SetBinding(InventoryCardView.LocationProperty, "Location");
        _EditableCard.SetBinding(InventoryCardView.LastEditedOnProperty, "LastEditedOn");
        _EditableCard.SetBinding(InventoryCardView.CreatedOnProperty, "CreatedOn");
        _EditableCard.HideKebab = true;
        _EditableCard.EditDescription += EditDescription;
        _EditableCard.EditQuantity += EditQuantity;
        _EditableCard.EditQuantityType += EditQuantityType;
        _EditableCard.EditStatus += EditStatus;
        _EditableCard.EditLocation += EditLocation;

        _ContentLayout.Add(_EditableCard.Row(0));
        _ContentLayout.Add(_AddButton.Row(1));

        Content = _ContentLayout;

        _AddButton.Clicked += Add;
    }

    private void EditLocation(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.PickLocation(
            LanguageService.Instance["Pick Location"],
            picked: (location) =>
            {
                _Inventory.LocationID = location.Id;
                _Inventory.Location = location.Description;
            },
            canceled: () => { }));
    }

    private void EditStatus(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.PickStatus(
            LanguageService.Instance["Pick Status"],
            picked: (status) =>
            {
                _Inventory.StatusID = status.Id;
                _Inventory.Status = status.Description; 
            },
            canceled: () => { }));
    }

    private void EditQuantityType(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.PickQtyType(
            LanguageService.Instance["Pick Quantity Type"],
            picked: (quantityType) =>
            {
                _Inventory.QtyTypeID = quantityType.Id;
                _Inventory.QuantityType = quantityType.Description;
            },
            canceled: () => { }));
    }

    private void EditQuantity(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Quantity"],
            _Inventory.Quantity.ToString(),
            Keyboard.Numeric,
            submitted: (text) =>
            {
                if (string.IsNullOrEmpty(text))
                    return;

                var isNumber = int.TryParse(text, out int newQty);
                if (!isNumber) 
                    return;  

                _Inventory.Quantity = newQty;
            },
            canceled: () => { }));
    }

    private void EditDescription(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(PageService.TakeUserInput(
            LanguageService.Instance["Edit Description"],
            _Inventory.Description,
            Keyboard.Plain,
            submitted: (text) =>
            {
                _Inventory.Description = text ?? "";
            },
            canceled: () => { }));
    }

    private async void Add(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
        _Add?.Invoke(_Inventory);
    }
}
