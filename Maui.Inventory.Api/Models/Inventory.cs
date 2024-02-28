namespace Maui.Inventory.Api.Models;

public class Inventory
{
    public int Id { get; set; } = 0;
    public int AdminId { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
    public string QuantityType { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime LastEditedOn { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class InventoryUpdate
{
    public Inventory? Inventory { get; set; }
    public Inventory? PreviousInventory { get; set; }
}
