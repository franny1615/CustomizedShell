﻿using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Inventory.Models;
using Maui.Components;
using Maui.Components.Interfaces;

namespace Maui.Inventory.ViewModels;

public partial class ProfileViewModel(
    ILanguageService languageService,
    IDAL<User> userDAL) : ObservableObject
{
    private readonly ILanguageService _LanguageService = languageService;
    private readonly IDAL<User> _UserDAL = userDAL;
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