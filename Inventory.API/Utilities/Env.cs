public static class Env
{
    public static string DatabaseConnection => String("DB_CS");
    public static string JWTIssuer => String("JWT_ISS");
    public static string JWTAudience => String("JWT_AUD");
    public static string JWTPrivateKey => String("JWT_KEY");
    public static string BusinessEmail => String("BUS_EML");
    public static string BusinessEmailPassword => String("BUS_PASS");

    public static string String(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? "";
    }
}