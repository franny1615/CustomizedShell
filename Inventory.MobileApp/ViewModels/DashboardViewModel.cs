using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using System.Text.Json;

namespace Inventory.MobileApp.ViewModels;

public class DashboardViewModel()
{
    public List<DashboardItem> DashboardItems { get; set; } = new List<DashboardItem>();

    public void LoadDashboard()
    {
        DashboardItems = new List<DashboardItem>
        {
            new DashboardItem
            {
                Type = DashboardItemType.Inventory,
            },
            new DashboardItem
            {
                Type = DashboardItemType.Employees,
            },
            new DashboardItem
            {
                Type = DashboardItemType.Statuses,
            },
            new DashboardItem
            {
                Type = DashboardItemType.Locations,
            },
            new DashboardItem
            {
                Type = DashboardItemType.QuantityTypes,
            },
            new DashboardItem
            {
                Type = DashboardItemType.Profile,
            }
        };
    }

    public void UpdateDashboardNames() // for localization purposes.
    {
        for (int i = 0; i < DashboardItems.Count; i++)
        {
            DashboardItems[i].UpdateName();
        }
    }

    public async Task LoadProfile()
    {
        var response = await NetworkService.Get<User>(Endpoints.userDetails, new Dictionary<string, string>());
        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            // TODO: log to cloud or something
#if DEBUG
            System.Diagnostics.Debug.WriteLine(response.ErrorMessage);
#endif
            return;
        }

        SessionService.CurrentUser = response.Data ?? new User();
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"PROFILE >>> {JsonSerializer.Serialize(SessionService.CurrentUser)}");
#endif

        var permissions = await NetworkService.Get<UserPermissions>(Endpoints.getPermissionByUser, new Dictionary<string, string>
        {
            { "userId", SessionService.CurrentUser.Id.ToString() }
        });
        if (!string.IsNullOrEmpty(permissions.ErrorMessage))
        {
            // TODO: log to cloud or something
#if DEBUG
            System.Diagnostics.Debug.WriteLine(response.ErrorMessage);
#endif
            return;
        };

        SessionService.CurrentPermissions = permissions.Data ?? new UserPermissions();
#if DEBUG
        System.Diagnostics.Debug.WriteLine($"PERMISSIONS >>> {JsonSerializer.Serialize(SessionService.CurrentPermissions)}");
#endif
    }
}
