using System.Text.Json.Serialization;
using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace Maui.Inventory.Models;

[Table("admin")]
public class Admin
{
    [PrimaryKey, Column("_id")]
    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [Column("username")]
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [Column("email")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Column("access_token")]
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [Column("license_id")]
    [JsonPropertyName("licenseID")]
    public int LicenseID { get; set; } = -1;

    [Column("is_dark_mode_on")]
    [JsonPropertyName("isDarkModeOn")]
    public bool IsDarkModeOn { get; set; } = false;

    [Column("password")]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [Column("is_license_valid")]
    [JsonPropertyName("isLicenseValid")]
    public bool IsLicenseValid { get; set; } = false;

    [Column("edit_inv_permissions")]
    [JsonPropertyName("editInventoryPermissions")]
    public int EditInventoryPermissions { get; set; } = 0;
}

public class AdminDAL : BaseDAL<Admin>, IDAL<Admin> { }
