using SQLite;
using System.Text.Json.Serialization;

namespace Maui.Inventory.Models;

[Table("status")]
public class Status
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
}
