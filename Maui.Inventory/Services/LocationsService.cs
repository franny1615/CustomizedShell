using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory.Services;

public class LocationsService(IAPIService apiService) : ILocationsService
{
    public async Task<ListNetworkResponse<Models.Location>> GetAll(ListRequest request, int adminId)
    {
        Dictionary<string, string> parameters = new()
        {
            { "Page", request.Page.ToString() },
            { "ItemsPerPage", request.ItemsPerPage.ToString() },
        };

        if (!string.IsNullOrEmpty(request.Search))
        {
            parameters.Add("Search", request.Search);
        }

        return await apiService.Get<ListNetworkResponse<Models.Location>>(Endpoint.GetLocations, parameters);
    }

    public async Task<bool> Insert(Models.Location itemToInsert)
    {
        return await apiService.Post<bool>(Endpoint.InsertLocations, itemToInsert);
    }

    public async Task<bool> Update(Models.Location itemToUpdate)
    {
        return await apiService.Post<bool>(Endpoint.UpdateLocations, itemToUpdate);
    }

    public async Task<bool> Delete(Models.Location itemToDelete)
    {
        return await apiService.Post<bool>(Endpoint.DeleteLocations, itemToDelete);
    }

    public async Task<string> GenerateBarcode(string code)
    {
        var result = await apiService.Get<object>(Endpoint.GenerateBarcode, new Dictionary<string, string>
        {
            { "Barcode", code }
        });
        return result.ToString();
    }
}
