using Maui.Inventory.Models;

namespace Maui.Inventory.Services.Interfaces;

public interface IAdminService
{
    Task<bool> Login(string username, string password);
    Task<RegistrationResponse> Register(string username, string password);
    Task<ListNetworkResponse<User>> GetUsers(ListRequest request);
    Task<bool> DeleteUser(User user);
    Task<bool> UpdateUser(User user);
}
