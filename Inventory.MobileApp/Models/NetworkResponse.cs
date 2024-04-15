namespace Inventory.MobileApp.Models;

public class NetworkResponse<T>
{
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}
