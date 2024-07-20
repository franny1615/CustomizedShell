using Inventory.API.Models;

namespace Inventory.API.Repositories;

public interface ICrudRepository<T>
{
    public Task<RepoResult<int>> Insert(T item, int companyId);
    public Task<RepoResult<bool>> Update(T item, int companyId);
    public Task<RepoResult<DeleteResult>> Delete(int itemId, int companyId);
    public Task<RepoResult<T>> Get(int id, int companyId);
    public Task<RepoResult<SearchResult<T>>> Get(SearchRequest request, int companyId);
}

public class SearchResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Total { get; set; } = 0;
}

public class SearchRequest
{
    public string Search { get; set; } = "";
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 20;
    public int InventoryItemID { get; set; } = -1;
}