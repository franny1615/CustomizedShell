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
}