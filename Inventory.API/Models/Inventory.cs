namespace Inventory.API.Models;

public class Inventory 
{
    public int Id { get; set; } = 0;
    public int CompanyId { get; set; } = 0; 
    public string Description { get; set; } = "";
    public string Barcode { get; set; } = "";
    public int Quantity { get; set; } = 0;
    
    public DateTime? LastEditedOn { get; set; } = null;
    public DateTime? CreatedOn { get; set; } = null;

    // EXPECTED ON GET
    public string QuantityType { get; set; } = "";
    public string Status { get; set; } = "";
    public string Location { get; set; } = "";

    // EXPECTED ON INSERT
    public int QtyTypeID { get; set; } = -1;
    public int LocationID { get; set; } = -1;
    public int StatusID { get; set; } = -1;
}