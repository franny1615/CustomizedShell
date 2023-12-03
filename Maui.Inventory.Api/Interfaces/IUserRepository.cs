using Maui.Inventory.Api.Models;

namespace Maui.Inventory.Api.Interfaces;

public interface IUserRepository
{
    public Task<bool> RegisterNewUser(
        string username,
        string password,
        bool isAdmin);

    public Task<AuthenticatedUser> AuthenticateUser(
        string username, 
        string password);
}
