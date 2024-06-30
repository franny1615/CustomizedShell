namespace Inventory.API.Models;

public class Inventory 
{
    public int Id { get; set; } = 0;
    public int CompanyId { get; set; } = 0; 
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public int Quantity { get; set; } = 0;
    public string QuantityType { get; set; } = "";
    public string Barcode { get; set; } = "";
    public string Location { get; set; } = "";
    public DateTime? LastEditedOn { get; set; } = null;
    public DateTime? CreatedOn { get; set; } = null;
}