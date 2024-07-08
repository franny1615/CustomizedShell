using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;
    [JsonPropertyName("companyID")]
    public int CompanyID { get; set; } = -1;
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    [JsonPropertyName("isDarkModeOn")]
    public bool IsDarkModeOn { get; set; } = false;
    [JsonPropertyName("localization")]
    public string Localization { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;
    [JsonPropertyName("isCompanyOwner")]
    public bool IsCompanyOwner { get; set; } = false;

    [JsonIgnore]
    public object PasswordHash { get; set; } = string.Empty;
    [JsonIgnore]
    public string Salt { get; set; } = string.Empty;

}
