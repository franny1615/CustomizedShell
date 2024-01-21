using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Inventory.Models.UserModels;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;
using Maui.Inventory.Models;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;
using Maui.Inventory.Models.AdminModels;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory.ViewModels.AdminVM;

public partial class AdminUsersViewModel : ObservableObject
{
    private const int ITEMS_PER_PAGE = 5;
    private readonly IAdminService _AdminService;
    private readonly ILanguageService _LanguageService;
    private readonly IUserService _UserService;
    private readonly IDAL<Admin> _AdminDAL;

    public ObservableCollection<User> Users { get; set; } = new();

    public MaterialPaginationModel PaginationModel = new();
    public MaterialEntryModel SearchModel = new();
    public User SelectedUser = null;

    public AdminEditUsersViewModel EditUsersViewModel => new(_LanguageService, _UserService, _AdminService, _AdminDAL, SelectedUser);

    public AdminUsersViewModel(
        IAdminService adminService,
        IUserService userService,
        IDAL<Admin> adminDAL,
        ILanguageService languageService)
    {
        _UserService = userService;
        _AdminDAL = adminDAL;
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
            Page = PaginationModel.CurrentPage - 1,
            ItemsPerPage = ITEMS_PER_PAGE,
            Search = SearchModel.Text,
        });

        for (int i = 0; i < users.Items.Count; i++)
        {
            Users.Add(users.Items[i]);
        }

        var addendum = users.Total % ITEMS_PER_PAGE == 0 ? 0 : 1;
        var division = users.Total / ITEMS_PER_PAGE; 
        var calculatedTotal = (users.Total % ITEMS_PER_PAGE == 0) ? division : division + addendum;

        PaginationModel.TotalPages = users.Total > ITEMS_PER_PAGE ? calculatedTotal : 1;
    }
}

public partial class AdminEditUsersViewModel : ObservableObject
{
    private readonly IDAL<Admin> _AdminDAL;
    private readonly IUserService _UserService;
    private readonly IAdminService _AdminService;
    private readonly ILanguageService _LanguageService;

    public MaterialEntryModel Username = new();
    public MaterialEntryModel Password = new();

    public EditMode EditMode = EditMode.NotSet;

    private User _SelectedUser = null;

    public AdminEditUsersViewModel(
        ILanguageService languageService,
        IUserService userService,
        IAdminService adminService,
        IDAL<Admin> adminDAL,
        User user = null)
    {
        _SelectedUser = user;

        _UserService = userService;
        _AdminService = adminService;
        _LanguageService = languageService;
        _AdminDAL = adminDAL;   

        Username.Placeholder = _LanguageService.StringForKey("Username");
        Username.PlaceholderIcon = MaterialIcon.Person;
        Username.Keyboard = Keyboard.Plain;

        Password.Placeholder = _LanguageService.StringForKey("Password");
        Password.PlaceholderIcon = MaterialIcon.Password;
        Password.Keyboard = Keyboard.Plain;
        Password.IsPassword = true;

        if (_SelectedUser != null)
        {
            EditMode = EditMode.Edit;
            Username.Text = _SelectedUser.UserName;
        }
        else
        {
            EditMode = EditMode.Add;
        }
    }

    public async Task<RegistrationResponse> RegisterUser()
    {
        Admin admin;
        try
        {
            admin = (await _AdminDAL.GetAll()).First();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);

            return RegistrationResponse.AdminDoesNotExist;
        }

        return await _UserService.Register(
            Username.Text,
            Password.Text,
            admin.Id);
    }

    public async Task<bool> EditUser()
    {
        if (_SelectedUser == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(Username.Text)) // can't leave user name empty
        {
            return false;
        }

        _SelectedUser.UserName = Username.Text;
        _SelectedUser.Password = Password.Text;

        return await _AdminService.UpdateUser(_SelectedUser);
    }

    public async Task<bool> DeleteUser()
    {
        if (_SelectedUser == null)
        {
            return false;
        }

        return await _AdminService.DeleteUser(_SelectedUser);
    }
}
