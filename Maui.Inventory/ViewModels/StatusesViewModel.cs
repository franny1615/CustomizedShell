using Maui.Inventory.Models;
using Maui.Components;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using Maui.Components.Utilities;
using System.Collections.ObjectModel;

namespace Maui.Inventory.ViewModels;

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
