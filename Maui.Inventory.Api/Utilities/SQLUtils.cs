using Dapper;
using Microsoft.Data.SqlClient;

namespace Maui.Inventory.Api.Utilities;

public static class SQLUtils
{
    public static async Task<IEnumerable<T>> QueryAsync<T>(string query)
    {
        string connectionString = Environment.GetEnvironmentVariable(Constants.INV_DB_CS) ?? "";
        using var connection = new SqlConnection(connectionString);

        return await connection.QueryAsync<T>(query);
    }
}
