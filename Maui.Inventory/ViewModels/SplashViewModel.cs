using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Utilities;

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

    public async Task<AccessMessage> IsAccessTokenValid()
        => await StringUtils.IsAccessTokenValid(_UserDAL, _AdminDAL);
}
