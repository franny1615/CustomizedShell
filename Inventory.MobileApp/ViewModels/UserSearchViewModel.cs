using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.ViewModels;

public class UserSearchViewModel : ISearchViewModel<User>
{
    private const int PAGE_SIZE = 15;

    public List<User> Items { get; set; } = new List<User>();
    public int TotalPages { get; set; } = 1;
    public int Total { get; set; } = 0;
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

        var response = await NetworkService.Get<SearchResult<User>>(Endpoints.userSearch, parameters);

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
        Total = total;

        if (TotalPages == 0)
            TotalPages = 1;
    }
}
