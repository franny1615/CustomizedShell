namespace Maui.Inventory.Api.Models;

public class Admin
{
    public int Id { get; set; } = -1;
    public string UserName { get; set; } = string.Empty;
    public BinaryData? PasswordHash { get; set; } = null;
    public string Salt { get; set; } = string.Empty;
    public int LicenseID { get; set; } = -1;
}

public class AdminRegistration
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthenticatedAdmin
{
    public string AccessToken { get; set; } = string.Empty;
}

