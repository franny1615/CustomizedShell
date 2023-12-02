using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace Maui.Inventory.Models;

[Table("inventory_item")]
public class InventoryItem : ISearchable
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

    [Ignore]
    public ImageSource Icon { get; set; } = "";

    [Ignore]
    public Color IconBackgroundColor { get; set; } = Application.Current.Resources["Primary"] as Color;

    public string[] SearchableTerms
    {
        get
        {
            string[] name = Name.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string[] desc = Description.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            List<string> words = [.. name, .. desc];

            return words.ToArray();
        }
    }
}

public class InventoryItemDAL : BaseDAL<InventoryItem>, IDAL<InventoryItem> { }