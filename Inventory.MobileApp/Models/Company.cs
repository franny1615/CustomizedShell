using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public class Company
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("address1")]
    public string Address1 { get; set; } = string.Empty;
    [JsonPropertyName("address2")]
    public string Address2 { get; set; } = string.Empty;
    [JsonPropertyName("address3")]
    public string Address3 { get; set; } = string.Empty;
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
    [JsonPropertyName("zip")]
    public string Zip { get; set; } = string.Empty;
    [JsonPropertyName("licenseExpiresOn")]
    public DateTime? LicenseExpiresOn { get; set; } = null;
}
