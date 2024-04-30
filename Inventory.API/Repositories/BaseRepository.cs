using Dapper;
using System.Data.SqlClient;

namespace Inventory.API.Repositories;

public class BaseRepository
{
    public async Task<IEnumerable<T>> QueryAsync<T>(string query)
    {
        string connectionString = Env.DatabaseConnection;
        using var connection = new SqlConnection(connectionString);

        return await connection.QueryAsync<T>(query);
    }
}
