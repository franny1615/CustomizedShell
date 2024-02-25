using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Utilities;

namespace Maui.Inventory.ViewModels;

public partial class AppViewModel : ObservableObject
{
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<Admin> _AdminDAL;

    public AppViewModel(
        IDAL<User> userDAL,
        IDAL<Admin> adminDAL)
    {
        _UserDAL = userDAL;
        _AdminDAL = adminDAL;
    }

    public async Task<bool> IsAccessTokenValid()
    {
        AccessMessage result = await StringUtils.IsAccessTokenValid(_UserDAL, _AdminDAL);
        return result == AccessMessage.AdminSignedIn || result == AccessMessage.UserSignedIn;
    }

    public async Task<bool> ShouldEnableDarkMode()
    {
        User user = (await _UserDAL.GetAll()).FirstOrDefault();
        Admin admin = (await _AdminDAL.GetAll()).FirstOrDefault();

        if (user is not null)
        {
            return user.IsDarkModeOn;
        }
        else if (admin is not null)
        {
            return admin.IsDarkModeOn;
        }

        return false;
    }

    public async Task<bool> IsLicenseValid()
    {
        User user = (await _UserDAL.GetAll()).FirstOrDefault();
        Admin admin = (await _AdminDAL.GetAll()).FirstOrDefault();

        if (user is not null)
        {
            return user.IsLicenseValid;
        }

        if (admin is not null)
        {
            return admin.IsLicenseValid;
        }

        return false;
    }

    public async Task<int> EditInventoryPermisssions()
    {
        User user = (await _UserDAL.GetAll()).FirstOrDefault();
        Admin admin = (await _AdminDAL.GetAll()).FirstOrDefault();

        if (user is not null)
        {
            return user.EditInventoryPermissions;
        }

        if (admin is not null)
        {
            return admin.EditInventoryPermissions;
        }

        return -1;
    }
}