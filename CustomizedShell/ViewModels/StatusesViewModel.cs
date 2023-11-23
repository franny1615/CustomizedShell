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
    public ImageSource CloseEditAddIcon { get; set; } = "close.png";
    public ImageSource AddIcon { get; set; } = "add.png";
    public CardStyle CardStyle { get; set; } = CardStyle.Mini;

    public bool ShowAdd => true;

    public ObservableCollection<ISearchable> Items { get; set; } = new();

    public StatusesViewModel(
        ILanguageService languageService,
        IDAL<Status> statusDAL)
    {
        _LanguageService = languageService;
        _StatusDAL = statusDAL;

        PageTitle = _LanguageService.StringForKey("Statuses");
        SearchPlaceholder = _LanguageService.StringForKey("Search");
    }

    public async Task GetAllItems(string search)
    {
        Items.Clear();
        (await _StatusDAL.GetAll())
            .Cast<ISearchable>()
            .ToList()
            .FilterList(search)
            .ForEach(Items.Add);
    }

    public Task<bool> Save(ISearchable searchable)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(ISearchable searchable)
    {
        throw new NotImplementedException();
    }
}
