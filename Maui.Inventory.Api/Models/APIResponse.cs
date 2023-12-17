namespace Maui.Inventory.Api.Models;

public class APIResponse<T>
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}
