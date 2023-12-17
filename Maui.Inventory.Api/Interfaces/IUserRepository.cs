using Maui.Inventory.Api.Models;

namespace Maui.Inventory.Api.Interfaces;

public interface IUserRepository
{
    public Task<APIResponse<UserResponse>> RegisterUser(
        int adminId,
        string username,
        string password);

    public Task<APIResponse<UserResponse>> RegisterAdmin(
        string username,
        string password);

    public Task<APIResponse<AuthenticatedUser>> AuthenticateUser(
        string username, 
        string password);

    public Task<APIResponse<AuthenticatedUser>> AuthenticateAdmin(
        string username,
        string password);
}
