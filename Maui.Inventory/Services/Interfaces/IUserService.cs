using Maui.Inventory.Models;

namespace Maui.Inventory.Services.Interfaces;

public interface IUserService
{
    Task<bool> Login(string username, string password);
    Task<RegistrationResponse> Register(string username, string password, int adminID);
}
