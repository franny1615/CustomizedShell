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
    private readonly MaterialImage _NoItemIcon = new()
    {
        Icon = MaterialIcon.Check_circle,
        IconColor = Application.Current.Resources["TextColor"] as Color,
        IconSize = 40
    };
    private readonly Label _NoItems = new()
    {
        FontSize = 21,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
    };
    private readonly VerticalStackLayout _NoItemsUI = new()
    {
        Spacing = 8,
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Center
    };
    #endregion

    #region Constructor
    public AdminStatusesPage(
        ILanguageService languageService,
        AdminStatusesViewModel statusesVM) : base(languageService)
    {
        BindingContext = statusesVM;

        _LangService = languageService;
        Title = _LangService.StringForKey("Statuses");

        _NoItems.Text = _LangService.StringForKey("No Statuses.");
        _NoItemsUI.Add(_NoItemIcon.Center());
        _NoItemsUI.Add(_NoItems);

        _Search = new(_NoItemsUI, new DataTemplate(() =>
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
        }), statusesVM);

        _ContentLayout.Children.Add(_Search.ZIndex(0));
        Content = _ContentLayout;
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
    #endregion
}
