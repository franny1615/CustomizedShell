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

    public async Task<bool> Login(int adminId, string username, string password)
    {
        var user = await _apiService.Post<User>(Endpoint.UserLogin, new
        {
            AdminId = adminId,
            UserName = username,
            Password = password
        });

        bool success = user != null && user.AccessToken.Length > 0;
        if (success)
        {
            await _adminDAL.DeleteAll();
            await _userDAL.DeleteAll();
            
            await _userDAL.Insert(user);
        }

        return success;
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
