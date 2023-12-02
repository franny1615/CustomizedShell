using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Inventory.Models;
using Maui.Components.Interfaces;

namespace Maui.Inventory.ViewModels;

public partial class DataViewModel(
    IDAL<Barcode> barcodeDAL,
    IDAL<Category> categoryDAL,
    IDAL<Status> statusDAL,
    IDAL<User> userDAL) : ObservableObject 
{
    private readonly IDAL<Barcode> _BarcodeDAL = barcodeDAL;
    private readonly IDAL<Category> _CategoryDAL = categoryDAL;
    private readonly IDAL<Status> _StatusDAL = statusDAL;
    private readonly IDAL<User> _UserDAL = userDAL;

    public async Task<int> GetBarcodeCount()
    {
        User currentUser = (await _UserDAL.GetAll()).FirstOrDefault(user => user.IsLoggedIn);
        List<Barcode> barcodes = await _BarcodeDAL.GetAll();
        List<Barcode> userBarcodes = new();
        barcodes.ForEach(code =>
        {
            if (code.UserID == currentUser.Id)
            {
                userBarcodes.Add(code);
            }
        });
        return userBarcodes.Count;
    }

    public async Task<int> GetCategoryCount()
    {
        User currentUser = (await _UserDAL.GetAll()).FirstOrDefault(user => user.IsLoggedIn);
        List<Category> categories = await _CategoryDAL.GetAll();
        List<Category> userCategories = new();
        categories.ForEach(category =>
        {
            if (category.UserID == currentUser.Id)
            {
                userCategories.Add(category);
            }
        });
        return userCategories.Count;
    }

    public async Task<int> GetStatusesCount()
    {
        User currentUser = (await _UserDAL.GetAll()).FirstOrDefault(user => user.IsLoggedIn);
        List<Status> statuses = await _StatusDAL.GetAll();
        List<Status> userStatuses = new();
        statuses.ForEach(status =>
        {
            if (status.UserID ==  currentUser.Id)
            {
                userStatuses.Add(status);
            }
        });
        return userStatuses.Count;
    }
}
