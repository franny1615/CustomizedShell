using Maui.Inventory.Models;

namespace Maui.Inventory.Services.Interfaces;

public interface IUserService
{
    Task<bool> Login(int adminId, string username, string password);
    Task<RegistrationResponse> Register(string username, string password, int adminID, int permissions);
}
