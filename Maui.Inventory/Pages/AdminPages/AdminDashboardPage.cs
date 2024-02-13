using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminDashboardPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private readonly ScrollView _scroll = new();
    private readonly VerticalStackLayout _contentLayout = new()
    {
        Padding = 16,
    };
    private readonly MaterialTile _locations = new()
    {
        Icon = MaterialIcon.Shelves,
        ForegroundColor = Colors.White,
        BackgroundColor = Application.Current.Resources["Primary"] as Color
    };
    private readonly MaterialTile _statuses = new()
    {
        Icon = MaterialIcon.Check_circle,
        ForegroundColor = Colors.White,
        BackgroundColor = Application.Current.Resources["Primary"] as Color
    };
    #endregion

    #region Constructor
    public AdminDashboardPage(ILanguageService languageService) : base(languageService)
    {
        Shell.SetTabBarIsVisible(this, true);

        _LangService = languageService;

        Title = _LangService.StringForKey("Data");

        _locations.Title = _LangService.StringForKey("Locations");
        _statuses.Title = _LangService.StringForKey("Statuses");

        _contentLayout.Add(new Grid
        {
            ColumnDefinitions = Columns.Define(Star, Star),
            ColumnSpacing = 8,
            Children =
            {
                _locations.Column(0),
                _statuses.Column(1),
            } 
        });

        _scroll.Content = _contentLayout;
        Content = _scroll;

        _locations.Clicked += LocationsClicked;
        _statuses.Clicked += StatusesClicked;
    }
    ~AdminDashboardPage()
    {
        _locations.Clicked -= LocationsClicked;
        _statuses.Clicked -= StatusesClicked;
    }
    #endregion

    #region Helpers
    private void StatusesClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(AdminStatusesPage));
    }

    private void LocationsClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(AdminLocationsPage));
    }
    #endregion
}