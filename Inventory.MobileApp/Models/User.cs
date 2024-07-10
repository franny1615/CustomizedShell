using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Inventory.MobileApp.Models;

public partial class User : ObservableObject
{
    [ObservableProperty]
    [property: JsonPropertyName("id")]
    public int id = -1;

    [ObservableProperty]
    [property: JsonPropertyName("companyID")]
    public int companyID = -1;

    [ObservableProperty]
    [property: JsonPropertyName("userName")]
    public string userName = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("password")]
    public string password = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("isDarkModeOn")]
    public bool isDarkModeOn = false;

    [ObservableProperty]
    [property: JsonPropertyName("localization")]
    public string localization = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("email")]
    public string email = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("phoneNumber")]
    public string phoneNumber = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("isCompanyOwner")]
    public bool isCompanyOwner = false;

    [JsonIgnore]
    public object PasswordHash = string.Empty;
    [JsonIgnore]
    public string Salt = string.Empty;

}
