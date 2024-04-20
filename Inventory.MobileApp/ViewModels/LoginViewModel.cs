using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.ViewModels;

public class LoginViewModel
{
    public async Task<bool> Login(string userName, string password)
    {
        var response = await NetworkService.Post<object>(Endpoints.login, new 
        {
            userName,
            password
        });
        SessionService.AuthToken = response.Data?.ToString() ?? "";
        return !string.IsNullOrEmpty(SessionService.AuthToken);
    }
}
