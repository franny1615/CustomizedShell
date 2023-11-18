using CustomizedShell.Models;

namespace CustomizedShell.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private UserDAL _UserDAL = new();
    private User _CurrentUser = new();

    public async Task<User> GetLoggedInUser()
    {
        _CurrentUser = (await _UserDAL.GetAll())?.First(user => user.IsLoggedIn);
        return _CurrentUser;
    }

    public async Task Save(
        string username, 
        string password, 
        string email)
    {
        _CurrentUser.Username = username;
        _CurrentUser.Password = password;
        _CurrentUser.Email = email;
        await _UserDAL.Save(_CurrentUser);
    } 

    public async Task DeleteAccount()
    {
        await _UserDAL.Delete(_CurrentUser);
        // TODO: delete all associated tables that are linked to this _CurrentUser id.
    }
}
