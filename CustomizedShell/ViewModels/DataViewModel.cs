using CustomizedShell.Models;

namespace CustomizedShell.ViewModels;

public partial class DataViewModel : BaseViewModel
{
    private readonly BarcodeDAL _BarcodeDAL = new();
    private readonly CategoryDAL _CategoryDAL = new();
    private readonly StatusDAL _StatusDAL = new();

    public async Task<int> GetBarcodeCount()
    {
        var barcodes = await _BarcodeDAL.GetAll();

        if (barcodes == null)
        {
            return 0;
        }

        return barcodes.Count;
    }

    public async Task<int> GetCategoryCount()
    {
        var categories = await _CategoryDAL.GetAll();

        if (categories == null)
        {
            return 0;
        }

        return categories.Count;
    }

    public async Task<int> GetStatusesCount()
    {
        var statuses = await _StatusDAL.GetAll();

        if (statuses == null)
        {
            return 0;
        }

        return statuses.Count;
    }
}
