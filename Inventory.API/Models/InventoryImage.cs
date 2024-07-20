namespace Inventory.API.Models;

public class InventoryImage
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int InventoryId { get; set; }
    public string ImageBase64 { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.MinValue;
}
