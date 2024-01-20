using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Inventory.Models.UserModels;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;
using Maui.Inventory.Models;
using Maui.Components;

namespace Maui.Inventory.ViewModels.AdminVM;

public partial class AdminUsersViewModel : ObservableObject
{
    private const int ITEMS_PER_PAGE = 20;
    private readonly IAdminService _AdminService;
    private readonly ILanguageService _LanguageService;

    public ObservableCollection<User> Users { get; set; } = new();

    [ObservableProperty]
    public int currentPage = 0;

    [ObservableProperty]
    public int totalPages = 1;

    public MaterialEntryModel SearchModel = new();

    public AdminUsersViewModel(
        IAdminService adminService,
        ILanguageService languageService)
    {
        _AdminService = adminService;
        _LanguageService = languageService;

        SearchModel.Placeholder = _LanguageService.StringForKey("Search");
        SearchModel.PlaceholderIcon = MaterialIcon.Search;
        SearchModel.Keyboard = Keyboard.Plain;
        SearchModel.EntryStyle = EntryStyle.Search;
    }

    public async Task GetUsers()
    {
        Users.Clear();

        var users = await _AdminService.GetUsers(new ListRequest
        {
            Page = CurrentPage,
            ItemsPerPage = ITEMS_PER_PAGE,
            Search = SearchModel.Text,
        });

        for (int i = 0; i < users.Items.Count; i++)
        {
            Users.Add(users.Items[i]);
        }

        var division = users.Total / ITEMS_PER_PAGE;
        var calculatedTotal = (users.Total % ITEMS_PER_PAGE == 0) ? division : division + 1;

        TotalPages = users.Total > ITEMS_PER_PAGE ? calculatedTotal : 1;
    }
}
