using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        User user = null;
        Admin admin = null;
        try
        {
            user = (await _UserDAL.GetAll()).First();
        }
        catch (Exception ex) { /* TODO: add logging */ }

        try
        {
            admin = (await _AdminDAL.GetAll()).First();
        }
        catch (Exception ex) { /* TODO: add logging */ }

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
            // TODO: add logging
            return false;
        }
    }
}