using Inventory.MobileApp.Services;
using System.ComponentModel;

namespace Inventory.MobileApp.Models;

public enum DashboardItemType
{
    Inventory,
    Employees,
    Statuses,
    Locations,
    QuantityTypes,
    Profile,
    Unknown
}

public class DashboardItem : INotifyPropertyChanged
{
    private DashboardItemType _Type = DashboardItemType.Unknown;
    public DashboardItemType Type
    {
        get => _Type;
        set
        {
            _Type = value;
            UpdateName();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
        }
    }

    private string _Name = string.Empty;
    public string Name
    {
        get => _Name;
        set
        {
            _Name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void UpdateName()
    {
        switch (_Type)
        {
            case DashboardItemType.Inventory:
                Name = LanguageService.Instance["Inventory"];
                break;
            case DashboardItemType.Employees:
                Name = LanguageService.Instance["Employees"];
                break;
            case DashboardItemType.Statuses:
                Name = LanguageService.Instance["Statuses"];
                break;
            case DashboardItemType.Locations:
                Name = LanguageService.Instance["Locations"];
                break;
            case DashboardItemType.QuantityTypes:
                Name = LanguageService.Instance["Quantity Types"];
                break;
            case DashboardItemType.Profile:
                Name = LanguageService.Instance["Profile"];
                break;
        }
    }
}
