using Inventory.API.Models;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.ViewModels;

public class StatusSearchViewModel : ISearchViewModel<Status>
{
    private const int PAGE_SIZE = 15;

    public List<Status> Items { get; set; } = new List<Status>();
    public int TotalPages { get; set; } = 1;
    public int Page { get; set; } = 0;

    public async Task Search(string search)
    {
        var parameters = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(search))
        {
            parameters.Add("Search", search);
        }
        parameters.Add("Page", Page.ToString());
        parameters.Add("PageSize", PAGE_SIZE.ToString());
        
        var response = await NetworkService.Get<SearchResult<Status>>(Endpoints.searchStatus, parameters);

        if (response == null)
            response = new();

        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            // TODO: log the error to the cloud or something.
            return;
        }

        Items = response.Data?.Items ?? [];
        int total = response.Data?.Total ?? 0;
        int addendum = total % PAGE_SIZE == 0 ? 0 : 1;
        TotalPages = (total / PAGE_SIZE) + addendum;

        if (TotalPages == 0)
            TotalPages = 1;
    }

    public async Task<NetworkResponse<int>> InsertStatus(string description)
    {
        return await NetworkService.Post<int>(Endpoints.insertStatus, new
        {
            Description = description,
        });
    }

    public async Task<NetworkResponse<bool>> DeleteStatus(int id)
    {
        return await NetworkService.Delete<bool>(Endpoints.deleteStatus, new Dictionary<string, string>
        {
            { "statusId", id.ToString() }
        });
    }
}
