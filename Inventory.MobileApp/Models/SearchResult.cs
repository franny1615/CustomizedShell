namespace Inventory.MobileApp.Models;

public class SearchResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Total { get; set; } = 0;
}
