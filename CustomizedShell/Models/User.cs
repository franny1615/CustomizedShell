using Maui.Components.DAL;
using SQLite;

namespace CustomizedShell.Models;

[Table("user")]
public class User
{
    [PrimaryKey, AutoIncrement, Column("_id")]
    public int Id { get; set; } = -1;

    [MaxLength(250), Unique]
    public string Username { get; set; }

    [MaxLength(250), Unique]
    public string Password { get; set; }

    public bool IsLoggedIn { get; set; }
}

public static class UserDAL
{
    private static async Task Init()
    {
        AppDB.Init();
        await AppDB.DBConnection.CreateTableAsync<User>();
    }

    public static async Task<List<User>> GetAll()
    {
        await Init();
        
        return await AppDB.DBConnection.Table<User>().ToListAsync();
    }

    public static async Task<long> Save(User user)
    {
        await Init();

        if (user.Id != -1)
        {
            await AppDB.DBConnection.UpdateAsync(user);
        }
        else
        {
            await AppDB.DBConnection.InsertAsync(user);
        }

        return user.Id;
    }

    public static async Task Delete(User user)
    {
        await Init();
        await AppDB.DBConnection.DeleteAsync(user);
    }
}