using SQLite;

namespace Maui.Components.DAL;

public static class AppDB
{
    private const string DatabaseFilename = "ApplicationDB.db3";
    private static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    public static SQLiteAsyncConnection DBConnection { get; private set; }

    public static void Init()
    {
        if (DBConnection is not null)
            return;

        var options = new SQLiteConnectionString(DatabasePath, true, "password", postKeyAction: c =>
        {
            c.Execute("PRAGMA cipher_compatability = 3");
        });

        DBConnection = new SQLiteAsyncConnection(options);
    }
}
