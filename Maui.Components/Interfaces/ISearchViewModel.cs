using System.Collections.ObjectModel;
using Maui.Components.Enums;

namespace Maui.Components.Interfaces;

public interface ISearchViewModel
{
    public string SearchPlaceholder { get; set; }
    public string PageTitle { get; set; }

    public ImageSource SearchIcon { get; set; }
    public ImageSource ClearSearchIcon { get; set; }
    public ImageSource AddIcon { get; set; }

    public EditSearchableArgs AddArgs { get; set; }
    public EditSearchableArgs EditArgs { get; set; }

    public CardStyle CardStyle { get; set; }
    public bool ShowAdd { get; }

    public ObservableCollection<ISearchable> Items { get; set; }

    public Task GetAllItems(string search);
    public Task<bool> Save(ISearchable searchable);
    public Task<bool> Delete(ISearchable searchable);
    public ISearchable NewSearchable();
}

public class EditSearchableArgs()
{
    public ImageSource DeleteIcon { get; set; }
    public ImageSource SaveIcon { get; set; }
    public ImageSource CloseIcon { get; set; }
    public string Title { get; set; } = string.Empty;
    public string NamePlaceholder { get; set; } = string.Empty;
    public string DescriptionPlaceholder { get; set; } = string.Empty;
    public string SavePlaceholder { get; set; } = string.Empty;
    public string DeletePlaceholder { get; set; } = string.Empty;

    
    public bool HasSaveConfirmation = false;
    public string SaveConfirmationTitle { get; set; } = string.Empty;
    public string SaveConfirmationMessage { get; set; } = string.Empty;
    public string ConfirmSave { get; set; } = string.Empty;
    public string DenySave { get; set; } = string.Empty;
    public string SaveErrorTitle { get; set; } = string.Empty;
    public string SaveErrorMessage {  get; set; } = string.Empty;
    public string SaveErrorAcknowledgement { get; set; } = string.Empty;

    
    public bool HasDeleteConfirmation = false;
    public string DeleteConfirmationTitle { get; set; } = string.Empty;
    public string DeleteConfirmationMessage { get; set; } = string.Empty;
    public string ConfirmDelete { get; set; } = string.Empty;
    public string DenyDelete { get; set; } = string.Empty;
    public string DeleteErrorTitle { get; set; } = string.Empty;
    public string DeleteErrorMessage { get; set; } = string.Empty;  
    public string DeleteErrorAcknowledgement { get; set; } = string.Empty;
}