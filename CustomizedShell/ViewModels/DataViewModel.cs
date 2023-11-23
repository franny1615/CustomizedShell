using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CustomizedShell.Models;
using Maui.Components.Interfaces;
using Maui.Components.Utilities;

namespace CustomizedShell.ViewModels;

public partial class DataViewModel : BaseViewModel
{
    private readonly InventoryItemDAL _InventoryDAL = new();
    private readonly BarcodeDAL _BarcodeDAL = new();
    private readonly CategoryDAL _CategoryDAL = new();
    private readonly StatusDAL _StatusDAL = new();

    public ObservableCollection<Status> Statuses { get; set; } = new();
    public ObservableCollection<Barcode> Barcodes { get; set; } = new();
    public ObservableCollection<Category> Categories { get; set; } = new();

    #region Barcode
    public async Task<int> GetBarcodeCount() => (await _BarcodeDAL.GetAll()).Count;
    #endregion

    #region Category
    public async Task<int> GetCategoryCount() => (await _CategoryDAL.GetAll()).Count;
    public async Task GetAllCategories(string search)
    {
        Categories.Clear();
        (await _CategoryDAL.GetAll())
            .Cast<ISearchable>()
            .ToList()
            .FilterList(search)
            .Cast<Category>()
            .ToList()
            .ForEach(Categories.Add);
    }
    #endregion

    #region Status
    public async Task GetAllStatuses(string search)
    {
        Statuses.Clear();
        (await _StatusDAL.GetAll())
            .Cast<ISearchable>()
            .ToList()
            .FilterList(search)
            .Cast<Status>()
            .ToList()
            .ForEach(Statuses.Add);
    }

    public async Task<bool> CanDeleteStatus(Status status)
    {
        // if linked to at least on item, status cannot be deleted
        var inventoryItem = (await _InventoryDAL.GetAll()).FirstOrDefault(inventory => inventory.StatusID == status.Id);
        return inventoryItem == null;
    }

    public async Task<int> GetStatusesCount() => (await _StatusDAL.GetAll()).Count;
    public async Task<bool> SaveStatus(Status status) => await _StatusDAL.Save(status);
    public async Task<bool> DeleteStatus(Status status) => await _StatusDAL.Delete(status);
    #endregion
}
