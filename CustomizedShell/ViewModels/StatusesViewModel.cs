using CustomizedShell.Models;
using Maui.Components;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using Maui.Components.Utilities;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;

namespace CustomizedShell.ViewModels;

public class StatusesViewModel : ISearchViewModel
{
    private readonly ILanguageService _LanguageService;
    private readonly IDAL<InventoryItem> _InventoryDAL;
    private readonly IDAL<Status> _StatusDAL;
    private readonly IDAL<User> _UserDAL;

    public string SearchPlaceholder { get; set; }
    public string PageTitle {  get; set; }
    public ImageSource SearchIcon { get; set; } = "search.png";
    public ImageSource ClearSearchIcon { get; set; } = "close.png";
    public ImageSource AddIcon { get; set; } = "add.png";
    public CardStyle CardStyle { get; set; } = CardStyle.Mini;

    public bool ShowAdd => true;

    public ObservableCollection<ISearchable> Items { get; set; } = new();

    public EditSearchableArgs AddArgs { get; set; } = new();
    public EditSearchableArgs EditArgs { get; set; } = new();

    public StatusesViewModel(
        ILanguageService languageService,
        IDAL<Status> statusDAL,
        IDAL<InventoryItem> inventoryDAL,
        IDAL<User> userDAL)
    {
        _LanguageService = languageService;
        _StatusDAL = statusDAL;
        _InventoryDAL = inventoryDAL;
        _UserDAL = userDAL;

        PageTitle = _LanguageService.StringForKey("Statuses");
        SearchPlaceholder = _LanguageService.StringForKey("Search");

        AddArgs.SaveIcon = "add.png";
        AddArgs.CloseIcon = "close.png";
        AddArgs.Title = _LanguageService.StringForKey("AddStatus");
        AddArgs.NamePlaceholder = _LanguageService.StringForKey("Status");
        AddArgs.DescriptionPlaceholder = _LanguageService.StringForKey("Description");
        AddArgs.SavePlaceholder = _LanguageService.StringForKey("Save");
        AddArgs.SaveErrorTitle = _LanguageService.StringForKey("ErrorOccurred");
        AddArgs.SaveErrorMessage = _LanguageService.StringForKey("ErrorMessage");
        AddArgs.SaveErrorAcknowledgement = _LanguageService.StringForKey("Ok");

        EditArgs.DeleteIcon = "trash.png";
        EditArgs.SaveIcon = "add.png";
        EditArgs.CloseIcon = "close.png";
        EditArgs.Title = _LanguageService.StringForKey("EditStatus");
        EditArgs.NamePlaceholder = _LanguageService.StringForKey("Status");
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
        EditArgs.SaveConfirmationMessage = _LanguageService.StringForKey("SaveStatusPrompt");
        EditArgs.ConfirmSave = _LanguageService.StringForKey("Save");
        EditArgs.DenySave = _LanguageService.StringForKey("Cancel");
        EditArgs.DeleteErrorTitle = _LanguageService.StringForKey("ErrorOccurred");
        EditArgs.DeleteErrorMessage = _LanguageService.StringForKey("CannotDeleteStatus") + "\n" + 
                                      _LanguageService.StringForKey("Or") + "\n" + 
                                      _LanguageService.StringForKey("ErrorMessage");
        EditArgs.DeleteErrorAcknowledgement = _LanguageService.StringForKey("Ok");
    }

    public async Task GetAllItems(string search)
    {
        Items.Clear();
        User currentUser = (await _UserDAL.GetAll()).FirstOrDefault(user => user.IsLoggedIn);

        List<Status> statuses = await _StatusDAL.GetAll();
        List<Status> userStatuses = new();
        statuses.ForEach(status =>
        {
            if (status.UserID == currentUser.Id)
            {
                userStatuses.Add(status);
            }
        });

        List<ISearchable> searchables = userStatuses.Cast<ISearchable>().ToList();
        List<ISearchable> filtered = searchables.FilterList(search);
        filtered.ForEach(Items.Add);
    }

    public async Task<bool> Save(ISearchable searchable)
    {
        User currentUser = (await _UserDAL.GetAll()).FirstOrDefault(user => user.IsLoggedIn);

        Status status = searchable as Status;
        status.UserID = currentUser.Id;

        return await _StatusDAL.Save(status);
    }

    public async Task<bool> Delete(ISearchable searchable)
    {
        bool linkedToInventory = (await _InventoryDAL.GetAll()).FirstOrDefault(inventoryItem => inventoryItem.StatusID == searchable.Id) != null;
        bool succesfullyDeleted = false;

        if (!linkedToInventory)
        {
            succesfullyDeleted = await _StatusDAL.Delete(searchable as Status);    
        }

        return !linkedToInventory && succesfullyDeleted;
    }

    public ISearchable NewSearchable() => new Status();
}
