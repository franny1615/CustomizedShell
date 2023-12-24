using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory.Services;

public class UserService : IUserService
{
    private readonly IDAL<User> _userDAL;
    private readonly IDAL<Admin> _adminDAL;
    private readonly IAPIService _apiService;

    public UserService(
        IDAL<User> userDAL,
        IDAL<Admin> adminDAL,
        IAPIService apiService)
    {
        _userDAL = userDAL;
        _adminDAL = adminDAL;
        _apiService = apiService;
    }

    public async Task<bool> Login(string username, string password)
    {
        var user = await _apiService.Post<User>(Endpoint.UserLogin, new
        {
            UserName = username,
            Password = password
        });

        if (user.AccessToken.Length > 0)
        {
            await _adminDAL.DeleteAll();
            await _userDAL.DeleteAll();
            await _userDAL.Save(user);
        }

        return user.AccessToken.Length > 0;
    }

    public async Task<RegistrationResponse> Register(
        string username, 
        string password,
        int adminID)
    {
        return await _apiService.Post<RegistrationResponse>(Endpoint.UserRegister, new
        {
            UserName = username,
            Password = password,
            AdminID = adminID
        });
    }
}
