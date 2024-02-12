using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace Maui.Inventory.Models;

public class Endpoint
{
    #region Admin
    public const string AdminLogin = "api/admin/login";
    public const string AdminRegister = "api/admin/register";
    public const string AdminUserDelete = "api/admin/deleteUser";
    public const string AdminGetAllUsers = "api/admin/users";
    public const string AdminUpdateUser = "api/admin/updateUser";
    public const string AdminUpdateSelf = "api/admin/updateAdmin";
    #endregion

    #region User
    public const string UserLogin = "api/user/login";
    public const string UserRegister = "api/user/register";
    #endregion

    #region Email
    public const string EmailStartVerifying = "api/email/beginValidation";
    public const string EmailVerify = "api/email/validate";
    #endregion

    #region Inventory
    public const string GetInventory = "api/inventory/list";
    public const string InsertInventory = "api/inventory/insert";
    public const string UpdateInventory = "api/inventory/update";
    public const string DeleteInventory = "api/inventory/delete";
    #endregion

    #region Locations
    public const string GetLocations  = "api/locations/list";
    public const string InsertLocations  = "api/locations/insert";
    public const string UpdateLocations = "api/locations/update";
    public const string DeleteLocations = "api/locations/delete";
    #endregion

    #region Status
    public const string GetStatus = "api/status/list";
    public const string InsertStatus = "api/status/insert";
    public const string UpdateStatus = "api/status/update";
    public const string DeleteStatus = "api/status/delete";
    #endregion
}

[Table("api_url")]
public class ApiUrl
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    [Column("url")]
    public string URL { get; set; } = "";
}

public class ApiUrlDAL : BaseDAL<ApiUrl>, IDAL<ApiUrl> { }