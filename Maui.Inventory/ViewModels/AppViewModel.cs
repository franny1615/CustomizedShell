using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Models.AdminModels;
using Maui.Inventory.Models.UserModels;
using Microsoft.AppCenter.Crashes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Maui.Inventory.ViewModels;

public partial class AppViewModel : ObservableObject
{
    private readonly IDAL<ApiUrl> _APIDAL;
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<Admin> _AdminDAL;

    public AppViewModel(
        IDAL<User> userDAL,
        IDAL<Admin> adminDAL,
        IDAL<ApiUrl> apiDAL)
    {
        _UserDAL = userDAL;
        _AdminDAL = adminDAL;
        _APIDAL = apiDAL;
    }

    public async Task<bool> IsAccessTokenValid()
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
            return IsJWTExpired(user.AccessToken);
        }
        else if (admin is not null)
        {
            return IsJWTExpired(admin.AccessToken);
        }
        else 
        {
            return false;
        }
    }

    private bool IsJWTExpired(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

        try
        {
            var expirationClaim = jsonToken.Claims.First(claim => claim.Type == "exp");
            var ticks = long.Parse(expirationClaim.Value);

            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;
            var now = DateTime.UtcNow;

            return tokenDate >= now;
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);

            return false;
        }
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