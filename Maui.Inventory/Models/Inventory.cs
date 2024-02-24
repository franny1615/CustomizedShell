using Maui.Components.DAL;
using Maui.Components.Interfaces;
using SQLite;
using System.Text.Json.Serialization;

namespace Maui.Inventory.Models;

[Table("inventory")]
public class Inventory
{
    [PrimaryKey, Column("_id")]
    [JsonPropertyName("id")]
    public int Id { get; set; } = -1;

    [Column("admin_id")]
    [JsonPropertyName("adminId")]
    public int AdminId { get; set; } = -1;

    [Column("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [Column("status")]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [Column("quantity")]
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 0;

    [Column("quantity_type")]
    [JsonPropertyName("quantityType")]
    public string QuantityType { get; set; } = string.Empty;

    [Column("barcode")]
    [JsonPropertyName("barcode")]
    public string Barcode { get; set; } = string.Empty;

    [Column("location")]
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [Column("last_edited_on")]
    [JsonPropertyName("lastEditedOn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? LastEditedOn { get; set; } = null;

    [Column("created_on")]
    [JsonPropertyName("createdOn")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedOn { get; set; } = null;

    [JsonIgnore]
    public string LastEditedOnStr
    {
        get
        {
            try
            {
                return LastEditedOn?.ToString("MM/dd/yyyy");
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    [JsonIgnore]
    public string CreatedOnStr
    {
        get
        {
            try
            {
                return CreatedOn?.ToString("MM/dd/yyyy");
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    [JsonIgnore]
    public string QuantityStr
    {
        get
        {
            return $"{Quantity} {QuantityType}";
        }
    }
}

public class InventoryDAL : BaseDAL<Inventory>, IDAL<Inventory> { }