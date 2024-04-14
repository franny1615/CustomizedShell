namespace Inventory.API.Models;

public class RepoResult<T>
{
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}
