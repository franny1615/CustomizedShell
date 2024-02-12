using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory.Services;

public class StatusService(IAPIService apiService) : ICRUDService<Status>
{
    public async Task<ListNetworkResponse<Status>> GetAll(ListRequest request, int adminId)
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

        return await apiService.Get<ListNetworkResponse<Status>>(Endpoint.GetStatus, parameters);
    }

    public async Task<bool> Insert(Status itemToInsert)
    {
        return await apiService.Post<bool>(Endpoint.InsertStatus, itemToInsert);
    }

    public async Task<bool> Update(Status itemToUpdate)
    {
        return await apiService.Post<bool>(Endpoint.UpdateStatus, itemToUpdate);
    }

    public async Task<bool> Delete(Status itemToDelete)
    {
        return await apiService.Post<bool>(Endpoint.DeleteStatus, itemToDelete);
    }
}
