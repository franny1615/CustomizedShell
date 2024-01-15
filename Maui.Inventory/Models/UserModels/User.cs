using System.Text.Json.Serialization;
using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace Maui.Inventory.Models.UserModels;

[Table("user")]
public class User
{
    [PrimaryKey, Column("_id")]
    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [Column("username")]
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [Column("access_token")]
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [Column("admin_id")]
    [JsonPropertyName("adminID")]
    public int AdminID { get; set; } = -1;

    [Column("is_dark_mode_on")]
    [JsonPropertyName("isDarkModeOn")]
    public bool IsDarkModeOn { get; set; } = false;

    [Column("password")]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [Column("is_license_valid")]
    [JsonPropertyName("isLicenseValid")]
    public bool IsLicenseValid { get; set; } = false;
}

public class UserDAL : BaseDAL<User>, IDAL<User> { }