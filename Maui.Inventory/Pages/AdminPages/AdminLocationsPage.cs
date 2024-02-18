using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.ViewModels.AdminVM;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminLocationsPage : BasePage
{
    #region Private Properties
    private AdminLocationsViewModel _ViewModel => (AdminLocationsViewModel) BindingContext;
    private readonly ILanguageService _LangService;
    private readonly MaterialList<Models.Location> _Search;
    #endregion

    #region Constructor
    public AdminLocationsPage(
        ILanguageService languageService,
        AdminLocationsViewModel viewModel) : base(languageService)
    {
        BindingContext = viewModel;
        _LangService = languageService;

        Title = _LangService.StringForKey("Locations");

        _Search = new(_LangService.StringForKey("No Locations."), MaterialIcon.Shelves, new DataTemplate(() =>
        {
            var view = new MaterialCardView();
            view.SetBinding(MaterialCardView.BindingContextProperty, ".");
            view.SetBinding(MaterialCardView.HeadlineProperty, "Description");
            view.SetBinding(MaterialCardView.SupportingTextProperty, "Barcode");
            view.Icon = MaterialIcon.Shelves;
            view.TrailingIcon = MaterialIcon.Chevron_right;
            view.IconColor = Colors.White;
            view.TrailingIconColor = Colors.White;
            view.TextColor = Colors.White;
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.Clicked += LocationClicked;

            return view;
        }), viewModel, isEditable: true);

        Content = _Search;

        _Search.AddItemClicked += AddLocation;
    }
    ~AdminLocationsPage()
    {
        _Search.AddItemClicked -= AddLocation;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.FetchPublic();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private void LocationClicked(object sender, EventArgs e)
    {
        // TODO: 
    }

    private void AddLocation(object sender, ClickedEventArgs e)
    {
        // TODO: 
    }
    #endregion
}
