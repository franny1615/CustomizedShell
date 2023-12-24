﻿using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace Maui.Inventory.Models;

[Table("user")]
public class User
{
    [PrimaryKey, Column("_id")]
    public int Id { get; set; } = -1;

    [Column("username")]
    public string UserName { get; set; } = string.Empty;

    [Column("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [Column("admin_id")]
    public int AdminID { get; set; } = -1;
}

public class UserDAL : BaseDAL<User>, IDAL<User> { }