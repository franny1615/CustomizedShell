using System.Reflection;

namespace Maui.Components.DAL;

public class BaseDAL<T> where T : new()
{
    private async Task Init()
    {
        AppDB.Init();
        await AppDB.DBConnection.CreateTableAsync<T>();
    }

    public async Task<List<T>> GetAll()
    {
        await Init();

        return await AppDB.DBConnection.Table<T>().ToListAsync();
    } 

    public async Task<bool> Save(T item)
    {
        await Init();

        int id = GetIDFromItem(item);
        if (id == -999)
        {
            return false;
        }
        else if (id == -1)
        {
            await AppDB.DBConnection.InsertAsync(item);
        }
        else 
        {
            await AppDB.DBConnection.UpdateAsync(item);
        }

        return true;
    }

    public async Task<bool> Delete(T item)
    {
        await Init();
        int id = GetIDFromItem(item);
        if (id != -999)
        {
            await AppDB.DBConnection.DeleteAsync(item);
            return true;
        }

        return false;
    }

    public async Task DeleteAll()
    {
        await Init();
        await AppDB.DBConnection.DeleteAllAsync<int>();
    }

    private int GetIDFromItem(T item)
    {
        var property = item.GetType().GetProperty("Id");
        if (property is PropertyInfo info &&
            info.GetValue(item) is int Id)
        {
            return Id;
        }

        return -999;
    }
}
