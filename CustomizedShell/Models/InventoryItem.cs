using Maui.Components.DAL;
using SQLite;

namespace CustomizedShell.Models;

[Table("inventory_item")]
public class InventoryItem
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    public int UserID { get; set; } = -1;

    public int CategoryID { get; set; } = -1;

    public int CodeID { get; set; } = -1;

    public int StatusID { get; set; } = -1;

    public int Quantity { get; set; } = 0;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class InventoryItemDAL : BaseDAL<InventoryItem> { }