using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public class Inventory 
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;
    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; } = 0;
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 0;
    [JsonPropertyName("quantityType")]
    public string QuantityType { get; set; } = "";
    [JsonPropertyName("barcode")]
    public string Barcode { get; set; } = "";
    [JsonPropertyName("location")]
    public string Location { get; set; } = "";
    [JsonPropertyName("lastEditedOn")]
    public DateTime? LastEditedOn { get; set; } = null;
    [JsonPropertyName("createdOn")]
    public DateTime? CreatedOn { get; set; } = null;
    [JsonPropertyName("qtyTypeID")]
    public int QtyTypeID { get; set; } = -1;
    [JsonPropertyName("locationID")]
    public int LocationID { get; set; } = -1;
    [JsonPropertyName("statusID")]
    public int StatusID { get; set; } = -1;
}