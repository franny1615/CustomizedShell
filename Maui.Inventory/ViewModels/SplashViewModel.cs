using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;

namespace Maui.Inventory.ViewModels;

public partial class SplashViewModel : ObservableObject
{
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<Admin> _AdminDAL;

    public SplashViewModel(
        IDAL<User> userDAL,
        IDAL<Admin> adminDAL)
    {
        _UserDAL = userDAL;
        _AdminDAL = adminDAL;
    }

    public async Task<bool> SomeoneHasLoggedInBefore()
    {
        bool loggedIn = false;
        try
        {
            bool admin = (await _AdminDAL.GetAll()).FirstOrDefault() != null;
            bool user = (await _UserDAL.GetAll()).FirstOrDefault() != null;
            
            loggedIn = admin || user;
        }
        catch (Exception ex)
        {
            // TODO: add logging
            System.Diagnostics.Debug.WriteLine("");
        }

        return loggedIn;
    }
}
