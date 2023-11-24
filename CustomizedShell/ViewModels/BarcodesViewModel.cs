using CustomizedShell.Models;
using Maui.Components;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using Maui.Components.Utilities;
using System.Collections.ObjectModel;

namespace CustomizedShell.ViewModels;

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

    public EditSearchableArgs AddArgs { get; set; } = new();
    public EditSearchableArgs EditArgs { get; set; } = new();

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

        AddArgs.SaveIcon = "add.png";
        AddArgs.CloseIcon = "close.png";
        AddArgs.Title = _LanguageService.StringForKey("AddBarcode");
        AddArgs.NamePlaceholder = _LanguageService.StringForKey("Barcode");
        AddArgs.DescriptionPlaceholder = _LanguageService.StringForKey("Description");
        AddArgs.SavePlaceholder = _LanguageService.StringForKey("Save");
        AddArgs.SaveErrorTitle = _LanguageService.StringForKey("ErrorOccurred");
        AddArgs.SaveErrorMessage = _LanguageService.StringForKey("ErrorMessage");
        AddArgs.SaveErrorAcknowledgement = _LanguageService.StringForKey("Ok");

        EditArgs.DeleteIcon = "trash.png";
        EditArgs.SaveIcon = "add.png";
        EditArgs.CloseIcon = "close.png";
        EditArgs.Title = _LanguageService.StringForKey("EditBarcode");
        EditArgs.NamePlaceholder = _LanguageService.StringForKey("Barcode");
        EditArgs.DescriptionPlaceholder = _LanguageService.StringForKey("Description");
        EditArgs.SavePlaceholder = _LanguageService.StringForKey("Save");
        EditArgs.DeletePlaceholder = _LanguageService.StringForKey("Delete");

        EditArgs.HasDeleteConfirmation = true;
        EditArgs.DeleteConfirmationTitle = _LanguageService.StringForKey("AreYouSure");
        EditArgs.DeleteConfirmationMessage = _LanguageService.StringForKey("DeletePrompt");
        EditArgs.ConfirmDelete = _LanguageService.StringForKey("Yes");
        EditArgs.DenyDelete = _LanguageService.StringForKey("No");
        EditArgs.SaveErrorTitle = _LanguageService.StringForKey("ErrorOccurred");
        EditArgs.SaveErrorMessage = _LanguageService.StringForKey("ErrorMessage");
        EditArgs.SaveErrorAcknowledgement = _LanguageService.StringForKey("Ok");

        EditArgs.HasSaveConfirmation = true;
        EditArgs.SaveConfirmationTitle = _LanguageService.StringForKey("AreYouSure");
        EditArgs.SaveConfirmationMessage = _LanguageService.StringForKey("SaveBarcodePrompt");
        EditArgs.ConfirmSave = _LanguageService.StringForKey("Save");
        EditArgs.DenySave = _LanguageService.StringForKey("Cancel");
        EditArgs.DeleteErrorTitle = _LanguageService.StringForKey("ErrorOccurred");
        EditArgs.DeleteErrorMessage = _LanguageService.StringForKey("CannotDeleteBarcode") + "\n" +
                                      _LanguageService.StringForKey("Or") + "\n" +
                                      _LanguageService.StringForKey("ErrorMessage");
        EditArgs.DeleteErrorAcknowledgement = _LanguageService.StringForKey("Ok");
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
