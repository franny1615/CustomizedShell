using Inventory.MobileApp.Models;

namespace Inventory.MobileApp.ViewModels;

public class DashboardViewModel()
{
    public List<DashboardItem> DashboardItems { get; set; } = new List<DashboardItem>();

    public async Task LoadDashboard()
    {
        // TODO: implement network request later...
        DashboardItems = new List<DashboardItem>
        {
            new DashboardItem
            {
                Type = DashboardItemType.Inventory,
                Count = 999999
            },
            new DashboardItem
            {
                Type = DashboardItemType.Employees,
                Count = 999999
            },
            new DashboardItem
            {
                Type = DashboardItemType.Statuses,
                Count = 999999
            },
            new DashboardItem
            {
                Type = DashboardItemType.Locations,
                Count = 999999
            },
            new DashboardItem
            {
                Type = DashboardItemType.QuantityTypes,
                Count = 999999
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
