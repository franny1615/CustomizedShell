using Inventory.API.Models;

namespace Inventory.API.Repositories.UserRegistration;

public interface IUserRepository
{
    Task<RepoResult<User>> GetUserById(int id);
    Task<RepoResult<int>> CreateUser(User user);
    Task<RepoResult<bool>> DoesUserNameExist(string userName);
    Task<RepoResult<bool>> UpdateUser(User user);
    Task<RepoResult<bool>> DeleteUser(User user);
    Task<RepoResult<string>> AuthenticateUser(User user);
}
