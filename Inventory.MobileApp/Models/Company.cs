using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Inventory.MobileApp.Models;

public partial class Company : ObservableObject
{
    [ObservableProperty]
    [property: JsonPropertyName("id")]
    public int id = -1;
    
    [ObservableProperty]
    [property: JsonPropertyName("name")]
    public string name = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("address1")]
    public string address1 = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("address2")]
    public string address2 = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("address3")]
    public string address3 = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("country")]
    public string country = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("city")]
    public string city = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("state")]
    public string state  = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("zip")]
    public string zip = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("licenseExpiresOn")]
    public DateTime? licenseExpiresOn = null;
}
