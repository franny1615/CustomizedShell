using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public class SearchResult<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = [];
    [JsonPropertyName("total")]
    public int Total { get; set; } = 0;
}
