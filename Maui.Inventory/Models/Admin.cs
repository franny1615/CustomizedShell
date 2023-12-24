using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace Maui.Inventory.Models;

[Table("admin")]
public class Admin
{
    [PrimaryKey, Column("_id")]
    public int Id { get; set; } = -1;

    [Column("username")]
    public string UserName { get; set; } = string.Empty;

    [Column("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [Column("license_id")]
    public int LicenseID { get; set; } = -1;
}

public class AdminDAL : BaseDAL<Admin>, IDAL<Admin> { }
