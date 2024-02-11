using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory.ViewModels.UserVM;

public partial class UserProfileViewModel : ObservableObject
{
    private readonly ILanguageService _LangService;
    private readonly IAdminService _AdminService;
    private readonly IDAL<User> _UserDAL;

    public MaterialEntryModel Username = new();
    public MaterialEntryModel AdminID = new(); 

    [ObservableProperty]
    public bool isDarkModeOn = false;

    private User _CurrentUser = null;

    public UserProfileViewModel(
        ILanguageService languageService,
        IAdminService adminService,
        IDAL<User> userDAL)
    {
        _LangService = languageService;
        _AdminService = adminService;
        _UserDAL = userDAL;

        Username.Placeholder = _LangService.StringForKey("Username");
        AdminID.Placeholder = _LangService.StringForKey("CompanyId");
        
        Username.PlaceholderIcon = MaterialIcon.Person;
        AdminID.PlaceholderIcon = MaterialIcon.Storefront;
    }

    public async Task GetProfile()
    {
        try
        {
            _CurrentUser = (await _UserDAL.GetAll()).First();
            Username.Text = _CurrentUser.UserName;
            AdminID.Text = _CurrentUser.AdminID.ToString();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }
    }

    public async Task Logout()
    {
        try
        {
            await _UserDAL.DeleteAll();
        }
        catch { }
    }

    public async Task<bool> Edit()
    {
        var users = await _UserDAL.GetAll();
        var user = users.First();

        user.IsDarkModeOn = IsDarkModeOn;

        await _UserDAL.Update(user);

        _CurrentUser.IsDarkModeOn = IsDarkModeOn;
        return await _AdminService.UpdateUser(_CurrentUser);
    }
}
