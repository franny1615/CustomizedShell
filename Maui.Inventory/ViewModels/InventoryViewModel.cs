using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using Maui.Inventory.ViewModels.AdminVM;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels;

public partial class InventoryViewModel : ObservableObject, IMaterialListVM<Models.Inventory>
{
    private readonly ILocationsService _locationService;
    public readonly IInventoryService InventoryService;
    private readonly IDAL<Admin> _adminDAL;
    private readonly IDAL<User> _userDAL;

    public readonly AdminLocationsViewModel LocationsVM;
    public readonly AdminStatusesViewModel StatusVM;
    public readonly AdminQuantityTypesViewModel QuantityTypesViewModel;

    public int ItemsPerPage { get; set; } = 5;
    public MaterialPaginationModel PaginationModel { get; set; } = new();
    public MaterialEntryModel SearchModel { get; set; } = new();
    public ObservableCollection<Models.Inventory> Items { get; set; } = new();

    public Models.Inventory SelectedCached = null;
    public Models.Inventory SelectedInventory = null;
    public EditMode EditMode => SelectedInventory == null ? EditMode.Add : EditMode.Edit;
    public string CurrentBarcodeBase64 { get; set; } = string.Empty;

    public MaterialEntryModel DescriptionModel { get; set; } = new();
    public MaterialEntryModel BarcodeModel { get; set; } = new();
    public MaterialEntryModel QuantityModel { get; set; } = new();
    public MaterialEntryModel LastEditenOn {  get; set; } = new();
    public MaterialEntryModel CreatedOn { get; set; } = new();
    public MaterialEntryModel StatusModel { get; set; } = new();
    public MaterialEntryModel QuantityTypeModel { get; set; } = new();
    public MaterialEntryModel LocationModel { get; set; } = new();

    public InventoryViewModel(
        IInventoryService inventoryService,
        ILanguageService languageService,
        ILocationsService locationService,
        IDAL<Admin> adminDAL,
        IDAL<User> userDAL,
        AdminLocationsViewModel locationsVM,
        AdminStatusesViewModel statusVM,
        AdminQuantityTypesViewModel quantityTypesViewModel)
    {
        InventoryService = inventoryService;
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

        StatusModel.Placeholder = languageService.StringForKey("Status");
        StatusModel.PlaceholderIcon = MaterialIcon.Check_circle;
        StatusModel.Keyboard = Keyboard.Plain;
        StatusModel.IsSpellCheckEnabled = false;

        QuantityTypeModel.Placeholder = languageService.StringForKey("Qty Type");
        QuantityTypeModel.PlaceholderIcon = MaterialIcon.Video_label;
        QuantityTypeModel.Keyboard = Keyboard.Plain;
        QuantityTypeModel.IsSpellCheckEnabled = false;

        LocationModel.Placeholder = languageService.StringForKey("Location");
        LocationModel.PlaceholderIcon = MaterialIcon.Shelves;
        LocationModel.Keyboard = Keyboard.Plain;
        LocationModel.IsSpellCheckEnabled = false;

        LocationsVM = locationsVM;
        StatusVM = statusVM;
        QuantityTypesViewModel = quantityTypesViewModel;
    }

    public async Task GetItems()
    {
        Items.Clear();

        var locations = await InventoryService.GetAll(new ListRequest
        {
            Page = PaginationModel.CurrentPage - 1,
            ItemsPerPage = ItemsPerPage,
            Search = SearchModel.Text
        });

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

        return await InventoryService.Delete(SelectedInventory);
    }

    public async Task<bool> Update()
    {
        if (SelectedInventory == null)
            return true;

        _ = int.TryParse(QuantityModel.Text, out int result);
        SelectedInventory.Description = DescriptionModel.Text;
        SelectedInventory.Quantity = result;
        SelectedInventory.Status = StatusVM.SelectedItems.FirstOrDefault()?.HeadLine ?? SelectedInventory.Status;
        SelectedInventory.QuantityType = QuantityTypesViewModel.SelectedItems.FirstOrDefault()?.HeadLine ?? SelectedInventory.QuantityType;
        SelectedInventory.Location = LocationsVM.SelectedItems.FirstOrDefault()?.HeadLine ?? SelectedInventory.Location;

        return await InventoryService.Update(SelectedInventory, SelectedCached);
    }

    public async Task<bool> Insert()
    {
        _ = int.TryParse(QuantityModel.Text, out int result);
        return await InventoryService.Insert(new Models.Inventory
        {
            AdminId = await GetAdminID(),
            Description = DescriptionModel.Text,
            Status = StatusVM.SelectedItems.FirstOrDefault().HeadLine ?? "",
            Quantity = result,
            QuantityType = QuantityTypesViewModel.SelectedItems.FirstOrDefault()?.HeadLine ?? "",
            Barcode = BarcodeModel.Text,
            Location = LocationsVM.SelectedItems.FirstOrDefault()?.HeadLine ?? "",
        });
    }

    private async Task<int> GetAdminID()
    {
        int adminID = -1;
        try
        {
            adminID = (await _adminDAL.GetAll()).First().Id;
        }
        catch { }
        try
        {
            adminID = (await _userDAL.GetAll()).First().AdminID;
        }
        catch { }
        return adminID;
    }

    public async Task GenerateBarcode(string code, string description)
    {
        string base64 = await _locationService.GenerateBarcode(description, code);
        if (!string.IsNullOrEmpty(base64))
        {
            CurrentBarcodeBase64 = base64;
        }
    }

    public void Clean()
    {
        DescriptionModel.Text = "";
        BarcodeModel.Text = "";
        QuantityModel.Text = "";
        StatusModel.Text = "";
        QuantityTypeModel.Text = "";
        LocationModel.Text = "";
        CurrentBarcodeBase64 = string.Empty;
        StatusVM.SelectedItems.Clear();
        LocationsVM.SelectedItems.Clear();
        QuantityTypesViewModel.SelectedItems.Clear();
    }

    public async Task<int> GetPermissions()
    {
        Admin admin = (await _adminDAL.GetAll()).FirstOrDefault();
        if (admin != null)
        {
            return (int)EditInventoryPerms.AdminAccess;
        }
        return await InventoryService.GetEditInventoryPermissions();
    }
}
