using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Models.AdminModels;
using Maui.Inventory.Models.UserModels;
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
        Admin admin = (await _adminDAL.GetAll()).First();

        Dictionary<string, string> parameters = new()
        {
            { "AdminID", $"{admin.Id}" },
            { "Quantities.Page", request.Page.ToString() },
            { "Quantities.ItemsPerPage", request.ItemsPerPage.ToString() },
            { "Quantities.Search", request.Search }
        };

        return await _apiService.Get<ListNetworkResponse<User>>(Endpoint.AdminGetAllUsers, parameters);
    }

    public async Task<bool> Login(string username, string pwd)
    {
        var admin = await _apiService.Post<Admin>(Endpoint.AdminLogin, new
        {
            userName = username,
            password = pwd
        });

        if (admin != null && admin.AccessToken.Length > 0)
        {
            await _userDAL.DeleteAll();
            await _adminDAL.DeleteAll();

            await _adminDAL.Insert(admin);
        }

        return admin != null && admin.AccessToken.Length > 0;
    }

    public async Task<RegistrationResponse> Register(
        string username, 
        string pwd,
        string eml,
        bool emlVerfied)
    {
        return await _apiService.Post<RegistrationResponse>(Endpoint.AdminRegister, new
        {
            userName = username,
            password = pwd,
            email = eml,
            emailVerified = emlVerfied
        });
    }

    public async Task<bool> UpdateUser(User user)
    {
        return await _apiService.Post<bool>(Endpoint.AdminUpdateUser, user);
    }

    public async Task<bool> UpdateAdmin(Admin admin)
    {
        return await _apiService.Post<bool>(Endpoint.AdminUpdateSelf, admin);
    }
}
