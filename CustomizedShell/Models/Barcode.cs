using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;

namespace CustomizedShell.Models;

[Table("barcode")]
public class Barcode : ISearchable
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    public int UserID { get; set; } = -1;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string[] SearchableTerms
    {
        get 
        {
            List<string> words = [.. Description.Split(" ", StringSplitOptions.RemoveEmptyEntries)];
            words.Add(Name); // name should always be a number

            return words.ToArray();
        }
    }

    [Ignore]
    public ImageSource Icon { get; set; } = "barcode.png";

    [Ignore]
    public Color IconBackgroundColor { get; set; } = Application.Current.Resources["Primary"] as Color;
}

public class BarcodeDAL : BaseDAL<Barcode>, IDAL<Barcode> { }