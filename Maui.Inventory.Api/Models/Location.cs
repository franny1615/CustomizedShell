namespace Maui.Inventory.Api.Models;

public class Location
{
    public int Id { get; set; }
    public int AdminId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
}
