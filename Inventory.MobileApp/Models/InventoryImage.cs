using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace Inventory.MobileApp.Models;

public partial class InventoryImage : ObservableObject
{
    [ObservableProperty]
    [property: JsonPropertyName("id")]
    public int id = -1;

    [ObservableProperty]
    [property: JsonPropertyName("companyId")]
    public int companyId = -1;

    [ObservableProperty]
    [property: JsonPropertyName("inventoryId")]
    public int inventoryId = -1;

    [ObservableProperty]
    [property: JsonPropertyName("imageBase64")]
    public string imageBase64 = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("createdOn")]
    public DateTime createdOn = DateTime.MinValue;
}
