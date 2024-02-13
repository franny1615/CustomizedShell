using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels.AdminVM;

public partial class AdminStatusesViewModel: ObservableObject, IMaterialListVM<Status>
{
    private readonly ICRUDService<Status> _statusService;
    private readonly ILanguageService _langService;
    private readonly IDAL<Admin> _adminDAL;
    private readonly IDAL<User> _userDAL;

    public int ItemsPerPage { get; set; } = 20;
    public MaterialPaginationModel PaginationModel { get; set; } = new();
    public MaterialEntryModel SearchModel { get; set; } = new();
    public ObservableCollection<Status> Items { get; set; } = new();

    public AdminStatusesViewModel(
        ICRUDService<Status> statusService,
        ILanguageService langService,
        IDAL<Admin> adminDAl,
        IDAL<User> userDAL)
    {
        _langService = langService;
        _statusService = statusService;
        _adminDAL = adminDAl;
        _userDAL = userDAL;

        SearchModel.Placeholder = _langService.StringForKey("Search");
        SearchModel.PlaceholderIcon = MaterialIcon.Search;
        SearchModel.Keyboard = Keyboard.Plain;
        SearchModel.EntryStyle = EntryStyle.Search;
    }

    public async Task GetItems()
    {
        Items.Clear();

        User user = (await _userDAL.GetAll()).FirstOrDefault();
        Admin admin = (await _adminDAL.GetAll()).FirstOrDefault();

        int adminId = -1;
        if(user != null)
        {
            adminId = user.AdminID;
        }
        else if (admin != null)
        {
            adminId = admin.Id;
        }

        var statuses = await _statusService.GetAll(new ListRequest
        {
            Page = PaginationModel.CurrentPage - 1,
            ItemsPerPage = ItemsPerPage,
            Search = SearchModel.Text
        }, adminId);

        var addendum = statuses.Total % ItemsPerPage == 0 ? 0 : 1;
        var division = statuses.Total / ItemsPerPage;
        var calculatedTotal = (statuses.Total % ItemsPerPage == 0) ? division : division + addendum;

        PaginationModel.TotalPages = statuses.Total > ItemsPerPage ? calculatedTotal : 1;
    }
}
