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
    #endregion

    #region User
    public const string UserLogin = "api/user/login";
    public const string UserRegister = "api/user/register";
    #endregion

    #region Email
    public const string EmailStartVerifying = "api/email/beginValidation";
    public const string EmailVerify = "api/email/validate";
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