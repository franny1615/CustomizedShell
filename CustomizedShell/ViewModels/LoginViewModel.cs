using CommunityToolkit.Mvvm.ComponentModel;
using CustomizedShell.Models;
using Maui.Components;
using Maui.Components.Interfaces;

namespace CustomizedShell.ViewModels;

public partial class LoginViewModel(
    ILanguageService languageService,
    IDAL<User> userDAL) : ObservableObject
{
    private readonly ILanguageService _LanguageService = languageService; 
    private readonly IDAL<User> _UserDAL = userDAL;

    public async Task<bool> Login(
        string username,
        string password
    )
    {
        var users = await _UserDAL.GetAll();
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
                await _UserDAL.Save(user);
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

        await _UserDAL.Save(user);
    }
}
