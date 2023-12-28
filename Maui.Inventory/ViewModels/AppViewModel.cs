using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;

namespace Maui.Inventory.ViewModels;

public partial class AppViewModel : ObservableObject
{
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<Admin> _AdminDAL;

    public AppViewModel(
        IDAL<User> userDAL,
        IDAL<Admin> adminDAL)
    {
        _UserDAL = userDAL;
        _AdminDAL = adminDAL;
    }

    public async Task<bool> AdminSignedIn()
    {
        return (await _AdminDAL.GetAll()).Count > 0;
    }

    public async Task<bool> UserSignedIn()
    {
        return (await _UserDAL.GetAll()).Count > 0;
    }
}