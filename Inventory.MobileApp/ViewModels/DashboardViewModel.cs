using Inventory.MobileApp.Models;

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
}
