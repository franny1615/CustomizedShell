namespace Inventory.API.Models;

public class Location
{
    public int Id { get; set; } = 0;
    public int CompanyId { get; set; } = 0;
    public string Description { get; set; } = "";
    public string Barcode { get; set; } = "";
}