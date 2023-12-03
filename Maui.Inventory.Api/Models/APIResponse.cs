namespace Maui.Inventory.Api.Models;

public class APIResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
}
