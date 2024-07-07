namespace Inventory.API.Models;

public class UserPermissions
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CompanyId { get; set; }
    public int InventoryPermissions { get; set; } 
}