using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace CustomizedShell.Models;

[Table("category")]
public class Category : ISearchable
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    public string Name { get; set; } = string.Empty;

    public string[] SearchableTerms => Name.Split(" ", StringSplitOptions.RemoveEmptyEntries);
}

public class CategoryDAL : BaseDAL<Category> { }
