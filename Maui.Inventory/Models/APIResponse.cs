namespace Maui.Inventory.Models;

public class APIResponse<T> where T : new()
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = new();
}

public class ListNetworkResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int Total { get; set; } = 0;
}

public class ListRequest
{
    public int Page { get; set; } = 0;
    public int ItemsPerPage { get; set; } = 10;
    public string Search { get; set; } = string.Empty;
}