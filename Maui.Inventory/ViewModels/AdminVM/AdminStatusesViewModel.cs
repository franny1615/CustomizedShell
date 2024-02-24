using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels.AdminVM;

public partial class AdminStatusesViewModel: ObservableObject, ISelectViewModel
{
    private readonly ICRUDService<Status> _statusService;
    private readonly ILanguageService _langService;
    private readonly IDAL<Admin> _adminDAL;

    public ObservableCollection<ISelectItem> SelectedItems { get; set; } = [];
    public string Title { get; set; } = "";
    public string NoItemsText { get; set; } = "";
    public string ItemsIcon { get; set; } = MaterialIcon.Check_circle;
    public SelectType SelectType { get; set; } = SelectType.SingleSelect;

    public int ItemsPerPage { get; set; } = 20;
    public MaterialPaginationModel PaginationModel { get; set; } = new();
    public MaterialEntryModel SearchModel { get; set; } = new();
    public ObservableCollection<ISelectItem> Items { get; set; } = new();

    public MaterialEntryModel DescriptionModel { get; set; } = new();

    public Status SelectedStatus = null;

    public EditMode EditMode => SelectedStatus == null ? EditMode.Add : EditMode.Edit;

    public AdminStatusesViewModel(
        ICRUDService<Status> statusService,
        ILanguageService langService,
        IDAL<Admin> adminDAl)
    {
        _langService = langService;
        _statusService = statusService;
        _adminDAL = adminDAl;

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

        Admin admin = (await _adminDAL.GetAll()).FirstOrDefault();

        int adminId = -1;
        if (admin != null)
        {
            adminId = admin.Id;
        }

        var statuses = await _statusService.GetAll(new ListRequest
        {
            Page = PaginationModel.CurrentPage - 1,
            ItemsPerPage = ItemsPerPage,
            Search = SearchModel.Text
        }, adminId);

        for (int i = 0; i < statuses.Items.Count; i++)
        {
            Items.Add(statuses.Items[i]);
        }

        var addendum = statuses.Total % ItemsPerPage == 0 ? 0 : 1;
        var division = statuses.Total / ItemsPerPage;
        var calculatedTotal = (statuses.Total % ItemsPerPage == 0) ? division : division + addendum;

        PaginationModel.TotalPages = statuses.Total > ItemsPerPage ? calculatedTotal : 1;
    }

    public async Task<bool> Update()
    {
        if (SelectedStatus == null)
            return true;
        if (string.IsNullOrEmpty(DescriptionModel.Text))
            return false;

        SelectedStatus.Description = DescriptionModel.Text;
        return await _statusService.Update(SelectedStatus);
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

        return await _statusService.Insert(new Status
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
            adminID = (await _adminDAL.GetAll()).First().Id;
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
        if (SelectedStatus == null)
            return true;

        return await _statusService.Delete(SelectedStatus);
    }
}
