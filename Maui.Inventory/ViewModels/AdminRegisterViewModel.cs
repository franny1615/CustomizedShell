using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory;

public partial class AdminRegisterViewModel : ObservableObject
{
    private readonly IAdminService _AdminService;

    public AdminRegisterViewModel(IAdminService adminService)
    {
        _AdminService = adminService;
    }
}
