using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Inventory.Services.Interfaces;
using System.Collections.ObjectModel;
using Maui.Inventory.Models;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Interfaces;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory.ViewModels.AdminVM;

public partial class AdminUsersViewModel : ObservableObject, IMaterialListVM<User>
{
    private readonly IAdminService _AdminService;
    private readonly ILanguageService _LanguageService;
    private readonly IUserService _UserService;
    private readonly IDAL<Admin> _AdminDAL;

    public int ItemsPerPage { get; set; } = 20;
    public ObservableCollection<User> Items { get; set; } = new();
    public MaterialPaginationModel PaginationModel { get; set; } = new();
    public MaterialEntryModel SearchModel { get; set; } = new();

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

    public async Task GetItems()
    {
        Items.Clear();

        var users = await _AdminService.GetUsers(new ListRequest
        {
            Page = PaginationModel.CurrentPage - 1,
            ItemsPerPage = ItemsPerPage,
            Search = SearchModel.Text,
        });

        for (int i = 0; i < users.Items.Count; i++)
        {
            Items.Add(users.Items[i]);
        }

        var addendum = users.Total % ItemsPerPage == 0 ? 0 : 1;
        var division = users.Total / ItemsPerPage; 
        var calculatedTotal = (users.Total % ItemsPerPage == 0) ? division : division + addendum;

        PaginationModel.TotalPages = users.Total > ItemsPerPage ? calculatedTotal : 1;
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
    public int EditInvPerms => _SelectedUser?.EditInventoryPermissions ?? -1;

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

    public async Task<RegistrationResponse> RegisterUser(int permissions)
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
            admin.Id,
            permissions);
    }

    public async Task<bool> EditUser(int permissions)
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
        _SelectedUser.EditInventoryPermissions = permissions;

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
