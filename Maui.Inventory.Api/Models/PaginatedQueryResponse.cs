namespace Maui.Inventory.Api.Models;

public class PaginatedQueryResponse<T> 
{
    public List<T> Items { get; set; } = [];
    public int Total { get; set; } = 0;
}

public class PaginatedRequest
{
    public int Page { get; set; } = 0;
    public int ItemsPerPage { get; set; } = 10;
    public string? Search { get; set; }
}

public class InventoryHistoryRequest : PaginatedRequest
{
    public int InventoryId { get; set; } = 0;
}
