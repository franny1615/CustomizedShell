namespace Maui.Inventory.Api.Models;

public class User
{
    public int Id { get; set; } = -1;
    public string UserName { get; set; } = string.Empty;
    public BinaryData? PasswordHash { get; set; } = null;
    public string Salt { get; set; } = string.Empty;
    public int AdminID { get; set; } = -1;
}

public class UserRegistration 
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int AdminID { get; set; } = -1;
}

public class AuthenticatedUser
{
    public string AccessToken { get; set; } = string.Empty;
}