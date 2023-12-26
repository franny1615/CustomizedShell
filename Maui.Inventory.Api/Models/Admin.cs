using System.Text.Json.Serialization;

namespace Maui.Inventory.Api.Models;

public class Admin
{
    public int Id { get; set; } = -1;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int LicenseID { get; set; } = -1;
    public string Password { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;

    [JsonIgnore]
    public BinaryData? PasswordHash { get; set; } = null;
    [JsonIgnore]
    public string Salt { get; set; } = string.Empty;
}

