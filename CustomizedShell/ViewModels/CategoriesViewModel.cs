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

    public string SearchPlaceholder { get; set; }
    public string PageTitle { get; set; }
    public ImageSource SearchIcon { get; set; } = "search.png";
    public ImageSource ClearSearchIcon { get; set; } = "close.png";
    public ImageSource AddIcon { get; set; } = "add.png";
    public CardStyle CardStyle { get; set; } = CardStyle.Mini;

    public bool ShowAdd => true;

    public ObservableCollection<ISearchable> Items { get; set; } = new();

    public EditSearchableArgs AddArgs { get; set; } = new();
    public EditSearchableArgs EditArgs { get; set; } = new();

    public CategoriesViewModel(
        ILanguageService languageService,
        IDAL<Category> categoryDAL)
    {
        _LanguageService = languageService;
        _CategoryDAL = categoryDAL;
        PageTitle = _LanguageService.StringForKey("Categories");
        SearchPlaceholder = _LanguageService.StringForKey("Search");

        AddArgs.SaveIcon = "";
        AddArgs.CloseIcon = "";
        AddArgs.Title = _LanguageService.StringForKey("");
        AddArgs.NamePlaceholder = _LanguageService.StringForKey("");
        AddArgs.DescriptionPlaceholder = _LanguageService.StringForKey("");
        AddArgs.SavePlaceholder = _LanguageService.StringForKey("");
        
        EditArgs.DeleteIcon = "";
        EditArgs.SaveIcon = "";
        EditArgs.CloseIcon = "";
        EditArgs.Title = _LanguageService.StringForKey("");
        EditArgs.NamePlaceholder = _LanguageService.StringForKey("");
        EditArgs.DescriptionPlaceholder = _LanguageService.StringForKey("");
        EditArgs.SavePlaceholder = _LanguageService.StringForKey("");
        EditArgs.DeletePlaceholder = _LanguageService.StringForKey("");
        EditArgs.HasDeleteConfirmation = true;
        EditArgs.DeleteConfirmationTitle = _LanguageService.StringForKey("");
        EditArgs.DeleteConfirmationMessage = _LanguageService.StringForKey("");
        EditArgs.ConfirmDelete = _LanguageService.StringForKey("");
        EditArgs.DenyDelete = _LanguageService.StringForKey("");
    }

    public async Task GetAllItems(string search)
    {
        Items.Clear();
        (await _CategoryDAL.GetAll())
            ?.Cast<ISearchable>()
            ?.ToList()
            ?.FilterList(search)
            ?.ForEach(Items.Add);
    }

    public Task<bool> Save(ISearchable searchable)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(ISearchable searchable)
    {
        throw new NotImplementedException();
    }

    public ISearchable NewSearchable() => new Category();
}
