using CommunityToolkit.Mvvm.ComponentModel;
using CustomizedShell.Models;
using Maui.Components.Interfaces;

namespace CustomizedShell.ViewModels;

public partial class DataViewModel(
    IDAL<Barcode> barcodeDAL,
    IDAL<Category> categoryDAL,
    IDAL<Status> statusDAL) : ObservableObject 
{
    private readonly IDAL<Barcode> _BarcodeDAL = barcodeDAL;
    private readonly IDAL<Category> _CategoryDAL = categoryDAL;
    private readonly IDAL<Status> _StatusDAL = statusDAL;

    public async Task<int> GetBarcodeCount() => (await _BarcodeDAL.GetAll()).Count;
    public async Task<int> GetCategoryCount() => (await _CategoryDAL.GetAll()).Count;
    public async Task<int> GetStatusesCount() => (await _StatusDAL.GetAll()).Count;
}
