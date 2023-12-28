using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;

namespace Maui.Inventory.ViewModels;

public partial class SplashViewModel : ObservableObject
{
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<Admin> _AdminDAL;
    private readonly IDAL<ApiUrl> _APIDAL;

    public SplashViewModel(
        IDAL<User> userDAL,
        IDAL<Admin> adminDAL,
        IDAL<ApiUrl> apiDAL)
    {
        _UserDAL = userDAL;
        _AdminDAL = adminDAL;
        _APIDAL = apiDAL;
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
        }

        return loggedIn;
    }

    public async void CheckAPIURL()
    {
        List<ApiUrl> apis = await _APIDAL.GetAll();
        if (apis == null || apis.Count == 0)
        {
            ApiUrl prod = new ApiUrl
            {
                URL = "https://mauiinventoryapi20231216094131.azurewebsites.net/"
            };
            await _APIDAL.Save(prod);
        }
    }
}
