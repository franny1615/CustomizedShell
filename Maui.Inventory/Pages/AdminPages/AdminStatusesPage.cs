using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.ViewModels.AdminVM;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminStatusesPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private AdminStatusesViewModel _ViewModel => (AdminStatusesViewModel)BindingContext;
    private readonly Grid _ContentLayout = new();
    private readonly MaterialList<Models.Status> _Search;
    #endregion

    #region Constructor
    public AdminStatusesPage(
        ILanguageService languageService,
        AdminStatusesViewModel statusesVM) : base(languageService)
    {
        BindingContext = statusesVM;

        _LangService = languageService;
        Title = _LangService.StringForKey("Statuses");

        _Search = new(noItemsText: _LangService.StringForKey("No Statuses."), noItemsIcon: MaterialIcon.Check_circle, new DataTemplate(() =>
        {
            var view = new MaterialCardView();
            view.SetBinding(MaterialCardView.BindingContextProperty, ".");
            view.SetBinding(MaterialCardView.HeadlineProperty, "Description");
            view.Icon = MaterialIcon.Check_circle;
            view.TrailingIcon = MaterialIcon.Chevron_right;
            view.IconColor = Colors.White;
            view.TrailingIconColor = Colors.White;
            view.TextColor = Colors.White;
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.Clicked += StatusClicked;

            return view;
        }), statusesVM, isEditable: true);

        _ContentLayout.Children.Add(_Search.ZIndex(0));
        Content = _ContentLayout;
        _Search.AddItemClicked += AddStatus;
    }
    ~AdminStatusesPage()
    {
        _Search.AddItemClicked -= AddStatus;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Search.Fetch();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private void StatusClicked(object sender, EventArgs e)
    {
        // TODO: 
    }

    private void AddStatus(object sender, ClickedEventArgs e)
    {
        // TODO:
    }
    #endregion
}
