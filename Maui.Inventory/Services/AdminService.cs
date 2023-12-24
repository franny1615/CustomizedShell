using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory.Services;

public class AdminService : IAdminService
{
    private readonly IDAL<Admin> _adminDAL;
    private readonly IDAL<User> _userDAL;

    private readonly IAPIService _apiService;

    public AdminService(
        IDAL<Admin> adminDAL,
        IDAL<User> userDAL,
        IAPIService apiService)
    {
        _adminDAL = adminDAL;
        _userDAL = userDAL;
        _apiService = apiService;
    }

    public async Task<bool> DeleteUser(User user)
    {
        return await _apiService.Post<bool>(Endpoint.AdminUserDelete, user);
    }

    public async Task<ListNetworkResponse<User>> GetUsers(ListRequest request)
    {
        Dictionary<string, string> parameters = new()
        {
            { "Page", request.Page.ToString() },
            { "ItemsPerPage", request.ItemsPerPage.ToString() },
            { "Search", request.Search }
        };

        return await _apiService.Get<ListNetworkResponse<User>>(Endpoint.AdminGetAllUsers, );
    }

    public async Task<bool> Login(string username, string password)
    {
        var admin = await _apiService.Post<Admin>(Endpoint.AdminLogin, new
        {
            UserName = username,
            Password = password
        });

        if (admin.AccessToken.Length > 0)
        {
            await _userDAL.DeleteAll();
            await _adminDAL.DeleteAll();

            await _adminDAL.Save(admin);
        }

        return admin.AccessToken.Length > 0;
    }

    public async Task<RegistrationResponse> Register(string username, string password)
    {
        return await _apiService.Post<RegistrationResponse>(Endpoint.AdminRegister, new
        {
            UserName = username,
            Password = password
        });
    }

    public async Task<bool> UpdateUser(User user)
    {
        return await _apiService.Post<bool>(Endpoint.AdminUpdateUser, user);
    }
}
