using CustomizedShell.Models;

namespace CustomizedShell.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    public async Task<bool> Login(
        string username,
        string password
    )
    {
        var users = await UserDAL.GetAll();
        if (users != null && users.Count > 0)
        {
            return await ValidateUser(users, username, password);
        }

        return false;
    }

    private async Task<bool> ValidateUser(
        List<User> users,
        string username, 
        string password)
    {
        foreach (var user in users)
        {
            if (user.Username == username.Trim() && user.Password == password)
            {
                user.IsLoggedIn = true;
                await UserDAL.Save(user);
                return true;
            }
        }

        return false;
    }

    public async Task RegisterUser(
        string username, 
        string password,
        string email
    )
    {
        User user = new ()
        {
            Username = username.Trim(),
            Password = password,
            Email = email,
            IsLoggedIn = true
        };

        // TODO: decide if you're going to save this in preferences to
        // determine who is logged in or not.
        var userID = await UserDAL.Save(user);
    }
}
