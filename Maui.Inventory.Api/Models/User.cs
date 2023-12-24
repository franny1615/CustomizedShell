using System.Text.Json.Serialization;

namespace Maui.Inventory.Api.Models;

public class User
{
    public int Id { get; set; } = -1;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int AdminID { get; set; } = -1;
    public string AccessToken { get; set; } = string.Empty;

    [JsonIgnore]
    public BinaryData? PasswordHash { get; set; } = null;
    [JsonIgnore]
    public string Salt { get; set; } = string.Empty;
}

public class UsersRequest
{
    public int AdminId { get; set; } = -1;
    public PaginatedRequest Quantities { get; set; } = new();
}