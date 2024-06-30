using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public class QuantityType
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;
    [JsonPropertyName("companyId")]
    public int CompanyId { get; set; } = 0;
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
}