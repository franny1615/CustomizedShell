using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory.Services;

public class QuantityTypesService(IAPIService apiService) : ICRUDService<QuantityType>
{
    public async Task<ListNetworkResponse<QuantityType>> GetAll(ListRequest request)
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

        return await apiService.Get<ListNetworkResponse<QuantityType>>(Endpoint.GetQuantityType, parameters);
    }

    public async Task<bool> Insert(QuantityType itemToInsert)
    {
        return await apiService.Post<bool>(Endpoint.InsertQuantityType, itemToInsert);
    }

    public async Task<bool> Update(QuantityType itemToUpdate)
    {
        return await apiService.Post<bool>(Endpoint.UpdateQuantityType, itemToUpdate);
    }

    public async Task<bool> Delete(QuantityType itemToDelete)
    {
        return await apiService.Post<bool>(Endpoint.DeleteQuantityType, itemToDelete);
    }
}
