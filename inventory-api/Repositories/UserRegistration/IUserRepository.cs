using inventory_api.Models;

namespace inventory_api.Repositories.UserRegistration;

public interface IUserRepository
{
    Task<User> GetUserById(int id);
    Task<int> CreateUser(User user);
    Task<bool> DoesUserNameExist(string userName);
    Task<bool> UpdateUser(User user);
    Task<bool> DeleteUser(User user);
    Task<string> AuthenticateUser(User user);
}
