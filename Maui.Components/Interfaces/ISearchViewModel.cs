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

    public CardStyle CardStyle { get; set; }
    public bool ShowAdd { get; }

    public ObservableCollection<ISearchable> Items { get; set; }

    public Task<int> GetItemCount();
    public Task GetAllItems(string search);
    public Task Save(ISearchable item);
    public Task Delete(ISearchable item);
}
