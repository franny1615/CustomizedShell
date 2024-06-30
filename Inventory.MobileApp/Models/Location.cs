using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public class Location
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;
    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; } = 0;
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
    [JsonPropertyName("barcode")]
    public string Barcode { get; set; } = "";
}