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
        string password,
        string email);

    public Task<APIResponse<User>> AuthenticateUser(
        int adminId,
        string username, 
        string password);

    public Task<APIResponse<Admin>> AuthenticateAdmin(
        string username,
        string password);

    public Task<APIResponse<PaginatedQueryResponse<User>>> GetUsersForAdmin(
        UsersRequest request);

    public Task<APIResponse<bool>> DeleteUserForAdmin(
        int adminId,
        int userId);

    public Task<APIResponse<bool>> EditUser(
        User user);

    public Task<APIResponse<bool>> BeginEmailVerification(string email);

    public Task<APIResponse<bool>> VerifyEmail(string email, int code);
}
