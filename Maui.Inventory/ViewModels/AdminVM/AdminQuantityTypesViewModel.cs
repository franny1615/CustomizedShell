using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels.AdminVM;

public partial class AdminQuantityTypesViewModel : ObservableObject, ISelectViewModel
{
    private readonly ICRUDService<QuantityType> _quantityTypeService;
    private readonly ILanguageService _langService;
    private readonly IDAL<Admin> _admin;

    public ObservableCollection<ISelectItem> SelectedItems { get; set; } = [];
    public string Title { get; set; } = "";
    public string NoItemsText { get; set; } = "";
    public string ItemsIcon { get; set; } = MaterialIcon.Video_label;
    public SelectType SelectType { get; set; } = SelectType.SingleSelect;

    public int ItemsPerPage { get; set; } = 20;
    public MaterialPaginationModel PaginationModel { get; set; } = new();
    public MaterialEntryModel SearchModel { get; set; } = new();
    public ObservableCollection<ISelectItem> Items { get; set; } = new();

    public MaterialEntryModel DescriptionModel { get; set; } = new();
    public QuantityType SelectedQtyType = null;

    public EditMode EditMode => SelectedQtyType == null ? EditMode.Add : EditMode.Edit;

    public AdminQuantityTypesViewModel(
        ICRUDService<QuantityType> quantityTypeService, 
        ILanguageService langService, 
        IDAL<Admin> admin)
    {
        _quantityTypeService = quantityTypeService;
        _langService = langService;
        _admin = admin;

        Title = _langService.StringForKey("Select Qty Type");
        NoItemsText = _langService.StringForKey("No Quantity Types.");

        SearchModel.Placeholder = _langService.StringForKey("Search");
        SearchModel.PlaceholderIcon = MaterialIcon.Search;
        SearchModel.Keyboard = Keyboard.Plain;
        SearchModel.EntryStyle = EntryStyle.Search;

        DescriptionModel.Placeholder = _langService.StringForKey("Description");
        DescriptionModel.PlaceholderIcon = MaterialIcon.Description;
        DescriptionModel.Keyboard = Keyboard.Plain;
        DescriptionModel.IsSpellCheckEnabled = false;
    }

    public async Task GetItems()
    {
        Items.Clear();

        Admin admin = (await _admin.GetAll()).FirstOrDefault();

        int adminId = -1;
        if (admin != null)
        {
            adminId = admin.Id;
        }

        var qtyTypes = await _quantityTypeService.GetAll(new ListRequest
        {
            Page = PaginationModel.CurrentPage - 1,
            ItemsPerPage = ItemsPerPage,
            Search = SearchModel.Text
        }, adminId);

        for (int i = 0; i < qtyTypes.Items.Count; i++)
        {
            Items.Add(qtyTypes.Items[i]);
        }

        var addendum = qtyTypes.Total % ItemsPerPage == 0 ? 0 : 1;
        var division = qtyTypes.Total / ItemsPerPage;
        var calculatedTotal = (qtyTypes.Total % ItemsPerPage == 0) ? division : division + addendum;

        PaginationModel.TotalPages = qtyTypes.Total > ItemsPerPage ? calculatedTotal : 1;
    }

    public async Task<bool> Update()
    {
        if (SelectedQtyType == null)
            return true;
        if (string.IsNullOrEmpty(DescriptionModel.Text))
            return false;

        SelectedQtyType.Description = DescriptionModel.Text;
        return await _quantityTypeService.Update(SelectedQtyType);
    }

    public async Task<bool> Insert()
    {
        if (string.IsNullOrEmpty(DescriptionModel.Text))
            return false;

        int adminID = await GetAdminID();
        if (adminID == -1)
        {
            return false;
        }

        return await _quantityTypeService.Insert(new QuantityType
        {
            AdminId = adminID,
            Description = DescriptionModel.Text,
        });
    }

    private async Task<int> GetAdminID()
    {
        int adminID = -1;
        try
        {
            adminID = (await _admin.GetAll()).First().Id;
        }
        catch { }
        return adminID;
    }

    public void Clean()
    {
        DescriptionModel.Text = "";
    }

    public async Task<bool> Delete()
    {
        if (SelectedQtyType == null)
            return true;

        return await _quantityTypeService.Delete(SelectedQtyType);
    }
}
