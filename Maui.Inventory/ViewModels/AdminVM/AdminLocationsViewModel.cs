using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels.AdminVM;

public partial class AdminLocationsViewModel : ObservableObject, ISelectViewModel
{
    private readonly ILocationsService _locationService;
    private readonly ILanguageService _langService;
    private readonly IDAL<Admin> _adminDAL;

    public ObservableCollection<ISelectItem> SelectedItems { get; set; } = [];
    public string Title { get; set; } = "";
    public string NoItemsText { get; set; } = "";
    public string ItemsIcon { get; set; } = MaterialIcon.Shelves;
    public SelectType SelectType { get; set; } = SelectType.SingleSelect;

    public int ItemsPerPage { get; set; } = 20;
    public MaterialPaginationModel PaginationModel { get; set; } = new();
    public MaterialEntryModel SearchModel { get; set; } = new();
    public ObservableCollection<ISelectItem> Items { get; set; } = new();

    public Models.Location SelectedLocation = null;

    public MaterialEntryModel DescriptionModel { get; set; } = new();
    public MaterialEntryModel BarcodeModel { get; set; } = new();

    public EditMode EditMode => SelectedLocation == null ? EditMode.Add : EditMode.Edit;
    public string CurrentBarcodeBase64 = string.Empty;

    public AdminLocationsViewModel(
        ILocationsService locationService,
        ILanguageService langService,
        IDAL<Admin> adminDAl)
    {
        _langService = langService;
        _locationService = locationService;
        _adminDAL = adminDAl;

        Title = _langService.StringForKey("Select Location");
        NoItemsText = _langService.StringForKey("No Locations.");

        SearchModel.Placeholder = _langService.StringForKey("Search");
        SearchModel.PlaceholderIcon = MaterialIcon.Search;
        SearchModel.Keyboard = Keyboard.Plain;
        SearchModel.EntryStyle = EntryStyle.Search;

        DescriptionModel.Placeholder = _langService.StringForKey("Description");
        DescriptionModel.PlaceholderIcon = MaterialIcon.Description;
        DescriptionModel.Keyboard = Keyboard.Plain;
        DescriptionModel.IsSpellCheckEnabled = false;

        BarcodeModel.Placeholder = _langService.StringForKey("Barcode");
        BarcodeModel.PlaceholderIcon = MaterialIcon.Looks_one;
        BarcodeModel.Keyboard = Keyboard.Plain;
        BarcodeModel.IsSpellCheckEnabled = false;
    }

    public async Task GetItems()
    {
        Items.Clear();

        var locations = await _locationService.GetAll(new ListRequest
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
        if (SelectedLocation == null)
            return true;

        return await _locationService.Delete(SelectedLocation);
    }

    public async Task<bool> Update()
    {
        if (SelectedLocation == null)
            return true;
        if (string.IsNullOrEmpty(DescriptionModel.Text))
            return false;

        SelectedLocation.Description = DescriptionModel.Text;
        return await _locationService.Update(SelectedLocation);
    }

    public async Task<bool> Insert()
    {
        if (string.IsNullOrEmpty(DescriptionModel.Text) || string.IsNullOrEmpty(BarcodeModel.Text))
        {
            return false;
        }

        int adminID = await GetAdminID();
        if (adminID == -1)
        {
            return false;
        }

        return await _locationService.Insert(new Models.Location
        {
            AdminId = adminID,
            Description = DescriptionModel.Text,
            Barcode = BarcodeModel.Text
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
        return adminID;    
    }

    public void Clean()
    {
        DescriptionModel.Text = "";
        BarcodeModel.Text = "";
        CurrentBarcodeBase64 = string.Empty;
    }

    public async Task GenerateBarcode(string code, string description)
    {
        string base64 = await _locationService.GenerateBarcode(description, code);
        if (!string.IsNullOrEmpty(base64))
        {
            CurrentBarcodeBase64 = base64;
        }
    }
}
