namespace Maui.Inventory.Models;

public enum EditInventoryPerms // these are for bitwise permission handling
{
    CanChangeStatus = 1,        // 0 0 0 0 0 0 1
    CanChangeDescription = 2,   // 0 0 0 0 0 1 0
    CanChangeQuantity = 4,      // 0 0 0 0 1 0 0
    CanChangeQuantityType = 8,  // 0 0 0 1 0 0 0
    CanChangeLocation = 16,     // 0 0 1 0 0 0 0
    CanDelete = 32,             // 0 1 0 0 0 0 0
    CanAddInventory = 64,       // 1 0 0 0 0 0 0
    AdminAccess = 127           // 1 1 1 1 1 1 1
}

public static class AccessControl
{
    public static bool IsLicenseValid { get; set; } = false;
    public static int EditInventoryPermissions { get; set; } = -1;

    public static bool CanChangeStatus(int permissions)
        => (EditInventoryPerms)(permissions & (int)EditInventoryPerms.CanChangeStatus) 
        == EditInventoryPerms.CanChangeStatus;

    public static bool CanChangeDescription(int permissions)
        => (EditInventoryPerms)(permissions & (int)EditInventoryPerms.CanChangeDescription)
        == EditInventoryPerms.CanChangeDescription;

    public static bool CanChangeQuantity(int permissions)
        => (EditInventoryPerms)(permissions & (int)EditInventoryPerms.CanChangeQuantity)
        == EditInventoryPerms.CanChangeQuantity;

    public static bool CanChangeQuantityType(int permissions)
        => (EditInventoryPerms)(permissions & (int)EditInventoryPerms.CanChangeQuantityType)
        == EditInventoryPerms.CanChangeQuantityType;

    public static bool CanChangeLocation(int permissions)
        => (EditInventoryPerms)(permissions & (int)EditInventoryPerms.CanChangeLocation)
        == EditInventoryPerms.CanChangeLocation;

    public static bool CanDeleteInventory(int permissions)
        => (EditInventoryPerms)(permissions & (int)EditInventoryPerms.CanDelete)
        == EditInventoryPerms.CanDelete;

    public static bool CanAddInventory(int permissions)
        => (EditInventoryPerms)(permissions & (int)EditInventoryPerms.CanAddInventory)
        == EditInventoryPerms.CanAddInventory;
}