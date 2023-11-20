using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CustomizedShell.Models;

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
    public async Task<int> GetBarcodeCount()
    {
        var barcodes = await _BarcodeDAL.GetAll();

        if (barcodes == null)
        {
            return 0;
        }

        return barcodes.Count;
    }
    #endregion

    #region Category
    public async Task<int> GetCategoryCount()
    {
        var categories = await _CategoryDAL.GetAll();

        if (categories == null)
        {
            return 0;
        }

        return categories.Count;
    }
    #endregion

    #region Status
    public async Task<int> GetStatusesCount()
    {
        var statuses = await _StatusDAL.GetAll();

        if (statuses == null)
        {
            return 0;
        }

        return statuses.Count;
    }

    public async Task GetAllStatuses(string search)
    {
        Statuses.Clear();

        List<Status> allStatus = await _StatusDAL.GetAll();
        List<Status> filtered = new();
        if (!string.IsNullOrEmpty(search))
        {
            foreach(var status in allStatus)
            {
                if (status.Name.ToLower().Contains(search.ToLower()))
                {
                    filtered.Add(status);
                }
            }
        }
        else
        {
            filtered.AddRange(allStatus);
        }

        foreach(var status in filtered)
        {
            Statuses.Add(status);
        }
    }

    public async Task<bool> SaveStatus(Status status)
    {
        return await _StatusDAL.Save(status);
    }

    public async Task<bool> CanDeleteStatus(Status status)
    {
        var inventory = await _InventoryDAL.GetAll();
        if (inventory == null)
        {
            return true;
        }

        if (inventory.Count == 0)
        {
            return true;
        }

        foreach(var item in inventory)
        {
            if (item.StatusID == status.Id)
            {
                return false; // cannot delete a status that has associated inventory attached to it
            }
        }

        return true;
    }

    public async Task<bool> DeleteStatus(Status status)
    {
        return await _StatusDAL.Delete(status);
    }
    #endregion
}
