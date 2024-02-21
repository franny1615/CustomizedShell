using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels;

public partial class InventoryViewModel : ObservableObject, IMaterialListVM<Models.Inventory>
{
    private readonly ILocationsService _locationService;
    private readonly ICRUDService<Models.Inventory> _inventoryService;
    private readonly IDAL<Admin> _adminDAL;
    private readonly IDAL<User> _userDAL;

    public int ItemsPerPage { get; set; } = 20;
    public MaterialPaginationModel PaginationModel { get; set; } = new();
    public MaterialEntryModel SearchModel { get; set; } = new();
    public ObservableCollection<Models.Inventory> Items { get; set; } = new();

    public Models.Inventory SelectedInventory = null;
    public EditMode EditMode => SelectedInventory == null ? EditMode.Add : EditMode.Edit;
    public string CurrentBarcodeBase64 { get; set; } = string.Empty;

    public MaterialEntryModel DescriptionModel { get; set; } = new();
    public MaterialEntryModel BarcodeModel { get; set; } = new();
    public MaterialEntryModel QuantityModel { get; set; } = new();
    public MaterialEntryModel LastEditenOn {  get; set; } = new();
    public MaterialEntryModel CreatedOn { get; set; } = new();

    public InventoryViewModel(
        ICRUDService<Models.Inventory> inventoryService,
        ILanguageService languageService,
        ILocationsService locationService,
        IDAL<Admin> adminDAL,
        IDAL<User> userDAL)
    {
        _inventoryService = inventoryService;
        _locationService = locationService;
        _adminDAL = adminDAL;
        _userDAL = userDAL;

        SearchModel.Placeholder = languageService.StringForKey("Search");
        SearchModel.PlaceholderIcon = MaterialIcon.Search;
        SearchModel.Keyboard = Keyboard.Plain;
        SearchModel.EntryStyle = EntryStyle.Search;

        DescriptionModel.Placeholder = languageService.StringForKey("Description");
        DescriptionModel.PlaceholderIcon = MaterialIcon.Description;
        DescriptionModel.Keyboard = Keyboard.Plain;
        DescriptionModel.IsSpellCheckEnabled = false;

        BarcodeModel.Placeholder = languageService.StringForKey("Barcode");
        BarcodeModel.PlaceholderIcon = MaterialIcon.Looks_one;
        BarcodeModel.Keyboard = Keyboard.Plain;
        BarcodeModel.IsSpellCheckEnabled = false;

        QuantityModel.Placeholder = languageService.StringForKey("Quantity");
        QuantityModel.PlaceholderIcon = MaterialIcon.Looks_one;
        QuantityModel.Keyboard = Keyboard.Numeric;
        QuantityModel.IsSpellCheckEnabled = false;

        LastEditenOn.Placeholder = languageService.StringForKey("Last Edited");
        LastEditenOn.PlaceholderIcon = MaterialIcon.Today;
        LastEditenOn.Keyboard = Keyboard.Plain;
        LastEditenOn.IsSpellCheckEnabled = false;

        CreatedOn.Placeholder = languageService.StringForKey("Created");
        CreatedOn.PlaceholderIcon = MaterialIcon.Today;
        CreatedOn.Keyboard = Keyboard.Plain;
        CreatedOn.IsSpellCheckEnabled = false;
    }

    public async Task GetItems()
    {
        Items.Clear();

        Admin admin = (await _adminDAL.GetAll()).FirstOrDefault();
        User user = (await _userDAL.GetAll()).FirstOrDefault();

        int adminId = -1;
        if (admin != null)
        {
            adminId = admin.Id;
        }
        if (user != null)
        {
            adminId = user.AdminID;
        }

        var locations = await _inventoryService.GetAll(new ListRequest
        {
            Page = PaginationModel.CurrentPage - 1,
            ItemsPerPage = ItemsPerPage,
            Search = SearchModel.Text
        }, adminId);

        for (int i = 0; i < locations.Items.Count; i++)
        {
            Items.Add(locations.Items[i]);
        }

        var addendum = locations.Total % ItemsPerPage == 0 ? 0 : 1;
        var division = locations.Total / ItemsPerPage;
        var calculatedTotal = (locations.Total % ItemsPerPage == 0) ? division : division + addendum;

        PaginationModel.TotalPages = locations.Total > ItemsPerPage ? calculatedTotal : 1;
    }

    public async Task<bool> Delete()
    {
        if (SelectedInventory == null)
            return true;

        return await _inventoryService.Delete(SelectedInventory);
    }

    public async Task<bool> Update()
    {
        if (SelectedInventory == null)
            return true;
        // TODO: run validations, populate updated values
        return await _inventoryService.Update(SelectedInventory);
    }

    public async Task<bool> Insert()
    {
        // TODO: do validations
        return false;
    }

    public async Task GenerateBarcode(string code)
    {
        string base64 = await _locationService.GenerateBarcode(code);
        if (!string.IsNullOrEmpty(base64))
        {
            CurrentBarcodeBase64 = base64;
        }
    }
}
