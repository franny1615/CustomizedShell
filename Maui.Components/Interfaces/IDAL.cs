namespace Maui.Components.Interfaces;

public interface IDAL<T>
{
    public Task<List<T>> GetAll();
    public Task<bool> Insert(T item);
    public Task<bool> Update(T item);
    public Task<bool> Save(T item);
    public Task<bool> Delete(T item);
    public Task DeleteAll();
}
