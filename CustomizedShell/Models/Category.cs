using Maui.Components.DAL;
using SQLite;

namespace CustomizedShell.Models;

[Table("category")]
public class Category
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    public string Name { get; set; } = string.Empty;
}

public class CategoryDAL : BaseDAL<Category> { }
