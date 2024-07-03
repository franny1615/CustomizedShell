using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Location = Inventory.MobileApp.Models.Location;

namespace Inventory.MobileApp.ViewModels;

public class LocationSearchViewModel : ISearchViewModel<Location>
{
    private Random RAND = new Random();
    private const int PAGE_SIZE = 15;

    public List<Location> Items { get; set; } = new List<Location>();
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

        var response = await NetworkService.Get<SearchResult<Location>>(Endpoints.searchLocation, parameters);

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

    public async Task<NetworkResponse<int>> InsertLocation(string description)
    {
        return await NetworkService.Post<int>(Endpoints.insertLocation, new
        {
            Description = description,
            Barcode = $"{RAND.Next(1000000, 9999999)}"
        });
    }

    public async Task<NetworkResponse<DeleteResult>> DeleteLocation(int id)
    {
        return await NetworkService.Delete<DeleteResult>(Endpoints.deleteLocation, new Dictionary<string, string>
        {
            { "locationId", id.ToString() } 
        });
    }

    public async Task<NetworkResponse<bool>> UpdateLocation(Location location)
    {
        return await NetworkService.Post<bool>(Endpoints.updateLocation, location);
    }
}
