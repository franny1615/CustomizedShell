using CustomizedShell.Models;
using Maui.Components;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using System.Collections.ObjectModel;
using Maui.Components.Utilities;

namespace CustomizedShell.ViewModels;

public class CategoriesViewModel : ISearchViewModel
{
    private readonly ILanguageService _LanguageService;
    private readonly IDAL<Category> _CategoryDAL;
    private readonly IDAL<InventoryItem> _InventoryDAL;
    private readonly IDAL<User> _UserDAL;

    public string SearchPlaceholder { get; set; }
    public string PageTitle { get; set; }
    public ImageSource SearchIcon { get; set; } = "search.png";
    public ImageSource ClearSearchIcon { get; set; } = "close.png";
    public ImageSource AddIcon { get; set; } = "add.png";
    public CardStyle CardStyle { get; set; } = CardStyle.Mini;

    public bool ShowAdd => true;

    public ObservableCollection<ISearchable> Items { get; set; } = new();

    public CategoriesViewModel(
        ILanguageService languageService,
        IDAL<Category> categoryDAL,
        IDAL<InventoryItem> inventoryDAL,
        IDAL<User> userDAL)
    {
        _LanguageService = languageService;
        _CategoryDAL = categoryDAL;
        _InventoryDAL = inventoryDAL;
        _UserDAL = userDAL;
        PageTitle = _LanguageService.StringForKey("Categories");
        SearchPlaceholder = _LanguageService.StringForKey("Search");
    }

    public async Task GetAllItems(string search)
    {
        Items.Clear();
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

        List<ISearchable> searchables = userCategories.Cast<ISearchable>().ToList();
        List<ISearchable> filtered = searchables.FilterList(search);
        filtered.ForEach(Items.Add);
    }

    public async Task<bool> Save(ISearchable searchable)
    {
        User currentUser = (await _UserDAL.GetAll()).FirstOrDefault(user => user.IsLoggedIn);

        Category category = searchable as Category;
        category.UserID = currentUser.Id;

        return await _CategoryDAL.Save(category);
    }

    public async Task<bool> Delete(ISearchable searchable)
    {
        bool linkedToInventory = (await _InventoryDAL.GetAll()).FirstOrDefault(inventoryItem => inventoryItem.CategoryID == searchable.Id) != null;
        bool successfullyDeleted = false;

        if (!linkedToInventory)
        {
            successfullyDeleted = await _CategoryDAL.Delete(searchable as Category);
        }

        return !linkedToInventory && successfullyDeleted;
    }

    public ISearchable NewSearchable() => new Category();
}
