using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.ViewModels;

public class InventoryImageSearchViewModel : ISearchViewModel<InventoryImage>
{
    private const int PAGE_SIZE = 5;

    public Models.Inventory InventoryItem { get; set; }

    public List<InventoryImage> Items { get; set; } = new List<InventoryImage>();
    public int TotalPages { get; set; } = 1;
    public int Page { get; set; } = 0;
    public int Total { get; set; } = 0;

    public InventoryImageSearchViewModel(Models.Inventory inventory)
    {
        InventoryItem = inventory; 
    }

    public async Task Search(string search)
    {
        var parameters = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(search))
        {
            parameters.Add("Search", search);
        }
        parameters.Add("InventoryItemID", InventoryItem.Id.ToString());
        parameters.Add("Page", Page.ToString());
        parameters.Add("PageSize", PAGE_SIZE.ToString());

        var response = await NetworkService.Get<SearchResult<InventoryImage>>(Endpoints.searchImage, parameters);

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

    public async Task<NetworkResponse<int>> InsertImage(InventoryImage image)
    {
        return await NetworkService.Post<int>(Endpoints.insertImage, image);
    }

    public async Task<NetworkResponse<DeleteResult>> DeleteImage(int id)
    {
        return await NetworkService.Delete<DeleteResult>(Endpoints.deleteImage, new Dictionary<string, string>
        {
            { "ID", id.ToString() }
        });
    }

    public async Task<NetworkResponse<bool>> UpdateImage(InventoryImage image)
    {
        return await NetworkService.Post<bool>(Endpoints.updateImage, image);
    }
}
