namespace Maui.Inventory.Api;

/// <summary>
/// Lists all environment variable names
/// that system should have in order for 
/// application to work properly
/// </summary>
public static class EnvironmentConstant
{
    /// <summary>
    /// Inventory Database Connection String
    /// </summary>
    public const string INV_DB_CS = "INV_DB_CS";

    /// <summary>
    /// Json Web Token Issuer
    /// </summary>
    public const string JWT_ISS = "JWT_ISS";

    /// <summary>
    /// Json Web Token Audience
    /// </summary>
    public const string JWT_AUD = "JWT_AUD";

    /// <summary>
    /// Json Web Token Private Key
    /// </summary>
    public const string JWT_KEY = "JWT_KEY";
}
