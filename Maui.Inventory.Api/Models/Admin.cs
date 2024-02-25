using System.Text.Json.Serialization;

namespace Maui.Inventory.Api.Models;

public class Admin
{
    public int Id { get; set; } = -1;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; } = false;
    public int LicenseID { get; set; } = -1;
    public string Password { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public bool IsDarkModeOn { get; set; } = false;
    public bool IsLicenseValid { get; set; } = false;
    public int EditInventoryPermissions { get; set; } = 0;

    [JsonIgnore]
    public BinaryData? PasswordHash { get; set; } = null;
    [JsonIgnore]
    public string Salt { get; set; } = string.Empty;
}

