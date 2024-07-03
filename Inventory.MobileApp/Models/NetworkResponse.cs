namespace Inventory.MobileApp.Models;

public class NetworkResponse<T>
{
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum DeleteResult
{
    LinkedToOtherItems = 0,
    SuccesfullyDeleted = 1
}