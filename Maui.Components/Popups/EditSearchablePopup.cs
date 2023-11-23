using CommunityToolkit.Maui.Views;
using Maui.Components.Controls;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Popups;

public class EditSearchablePopup : Popup
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly ISearchViewModel _SearchViewModel;
    private readonly ISearchable _Searchable;
    private readonly Grid _ContentLayout = new()
    {
        ColumnDefinitions = Columns.Define(Star, 60),
        RowSpacing = 8,
        ColumnSpacing = 8,
        Padding = 16
    };
    private readonly Label _TitleLabel = new()
    {
        FontSize = 21,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly Image _CloseIcon = new()
    {
        HeightRequest = 30,
        WidthRequest = 30,
    };
    private readonly StyledEntry _NameEntry = new();
    private readonly StyledEntry _DescriptionEntry = new();
    private readonly FloatingActionButton _SaveButton = new();
    private readonly FloatingActionButton _DeleteButton = new();
    #endregion

    #region Constructor
    public EditSearchablePopup(
        ILanguageService languageService,
        ISearchable searchable,
        ISearchViewModel searchViewModel,
        CardStyle cardStyle)
    {
        _LanguageService = languageService;
        _Searchable = searchable;
        _SearchViewModel = searchViewModel;


    }
    #endregion
}
