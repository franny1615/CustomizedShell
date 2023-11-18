using Maui.Components.DAL;
using SQLite;

namespace CustomizedShell.Models;

[Table("barcode")]
public class Barcode
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    public int Number { get; set; } = -1;

    public string Description { get; set; } = string.Empty;
}

public class BarcodeDAL : BaseDAL<Barcode> { }