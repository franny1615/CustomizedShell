﻿using System.Text.Json.Serialization;

namespace Maui.Inventory.Api.Models;

public class User
{
    public int Id { get; set; } = -1;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int AdminID { get; set; } = -1;
    public string AccessToken { get; set; } = string.Empty;
    public bool IsDarkModeOn { get; set; } = false;
    public bool IsLicenseValid { get; set; } = false;
    public int EditInventoryPermissions { get; set; } = 0;

    [JsonIgnore]
    public BinaryData? PasswordHash { get; set; } = null;
    [JsonIgnore]
    public string Salt { get; set; } = string.Empty;
}