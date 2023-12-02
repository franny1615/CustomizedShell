using Maui.Inventory.Models;
using Maui.Components;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using Maui.Components.Utilities;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels;

public class BarcodesViewModel : ISearchViewModel
{
    private readonly ILanguageService _LanguageService;
    private readonly IDAL<InventoryItem> _InventoryDAL;
    private readonly IDAL<User> _UserDAL;
    private readonly IDAL<Barcode> _BarcodeDAL;

    public string SearchPlaceholder { get; set; }
    public string PageTitle { get; set; }
    public ImageSource SearchIcon { get; set; } = "search.png";
    public ImageSource ClearSearchIcon { get; set; } = "close.png";
    public ImageSource AddIcon { get; set; } = "add.png";
    public CardStyle CardStyle { get; set; } = CardStyle.Regular;

    public bool ShowAdd => true;

    public ObservableCollection<ISearchable> Items { get; set; } = new();

    public BarcodesViewModel(
        ILanguageService languageService,
        IDAL<InventoryItem> inventoryDAL,
        IDAL<User> userDAL,
        IDAL<Barcode> barcodeDAL)
    {
        _LanguageService = languageService;
        _BarcodeDAL = barcodeDAL;
        _InventoryDAL = inventoryDAL;
        _UserDAL = userDAL;

        PageTitle = _LanguageService.StringForKey("Barcodes");
        SearchPlaceholder = _LanguageService.StringForKey("Search");
    }

    public async Task<bool> Delete(ISearchable searchable)
    {
        bool linkedToInventory = (await _InventoryDAL.GetAll()).FirstOrDefault(inventoryItem => inventoryItem.CodeID == searchable.Id) != null;
        bool successfullyDeleted = false;

        if (!linkedToInventory)
        {
            successfullyDeleted = await _BarcodeDAL.Delete(searchable as Barcode);
        }

        return !linkedToInventory && successfullyDeleted;
    }

    public async Task GetAllItems(string search)
    {
        Items.Clear();
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

        List<ISearchable> searchables = userBarcodes.Cast<ISearchable>().ToList();
        List<ISearchable> filtered = searchables.FilterList(search);
        filtered.ForEach(Items.Add);
    }

    public ISearchable NewSearchable() => new Barcode();

    public async Task<bool> Save(ISearchable searchable)
    {
        User currentUser = (await _UserDAL.GetAll()).FirstOrDefault(user => user.IsLoggedIn);

        Barcode code = searchable as Barcode;
        code.UserID = currentUser.Id;

        return await _BarcodeDAL.Save(code);
    }
}
