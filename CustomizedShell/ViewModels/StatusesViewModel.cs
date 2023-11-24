using CustomizedShell.Models;
using Maui.Components;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using Maui.Components.Utilities;
using System.Collections.ObjectModel;

namespace CustomizedShell.ViewModels;

public class StatusesViewModel : ISearchViewModel
{
    private readonly ILanguageService _LanguageService;
    private readonly IDAL<Status> _StatusDAL;

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
        IDAL<Status> statusDAL)
    {
        _LanguageService = languageService;
        _StatusDAL = statusDAL;

        PageTitle = _LanguageService.StringForKey("Statuses");
        SearchPlaceholder = _LanguageService.StringForKey("Search");

        AddArgs.SaveIcon = "add.png";
        AddArgs.CloseIcon = "close.png";
        AddArgs.Title = _LanguageService.StringForKey("AddStatus");
        AddArgs.NamePlaceholder = _LanguageService.StringForKey("Status");
        AddArgs.DescriptionPlaceholder = _LanguageService.StringForKey("Description");
        AddArgs.SavePlaceholder = _LanguageService.StringForKey("Save");
        
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
    }

    public async Task GetAllItems(string search)
    {
        Items.Clear();
        List<Status> statuses = await _StatusDAL.GetAll();
        List<ISearchable> searchables = statuses.Cast<ISearchable>().ToList();
        List<ISearchable> filtered = searchables.FilterList(search);
        filtered.ForEach(Items.Add);
    }

    public Task<bool> Save(ISearchable searchable)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(ISearchable searchable)
    {
        throw new NotImplementedException();
    }

    public ISearchable NewSearchable() => new Status();
}
