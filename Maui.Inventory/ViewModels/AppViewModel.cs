using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Models.AdminModels;
using Maui.Inventory.Models.UserModels;
using Maui.Inventory.Utilities;
using Microsoft.AppCenter.Crashes;

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
        User user = null;
        Admin admin = null;
        try
        {
            user = (await _UserDAL.GetAll()).First();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }

        try
        {
            admin = (await _AdminDAL.GetAll()).First();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }

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
}