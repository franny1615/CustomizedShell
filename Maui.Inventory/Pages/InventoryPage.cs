using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;

namespace Maui.Inventory.Pages;

public class InventoryPage : BasePage
{
	#region Private Properties
	private InventoryViewModel _viewModel => (InventoryViewModel) BindingContext;
	private readonly ILanguageService _languageService;
	private readonly MaterialList<Models.Inventory> _Search;
    #endregion

    #region Constructor
    public InventoryPage(
		ILanguageService languageService,
		InventoryViewModel viewModel) : base(languageService)
	{
		_languageService = languageService;
		BindingContext = viewModel;

		Title = _languageService.StringForKey("Inventory");

		_Search = new(_languageService.StringForKey("No Inventory."), MaterialIcon.Inventory_2, new DataTemplate(() =>
		{
			var view = new MaterialArticleCardView();
			view.SetBinding(MaterialArticleCardView.BindingContextProperty, ".");
			view.SetBinding(MaterialArticleCardView.ArticleProperty, "Description");
            view.SetBinding(MaterialArticleCardView.MainSupportOneProperty, "Location");
            view.SetBinding(MaterialArticleCardView.SecondarySupportTwoProperty, "QuantityStr");
            view.SetBinding(MaterialArticleCardView.SecondarySupportOneProperty, "Status");
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.TextColor = Colors.White;

            view.Clicked += EditInventory;

			return view;
		}), viewModel, isEditable: AccessControl.IsLicenseValid);

		Content = _Search;

        _Search.AddItemClicked += AddInventory;
	}
	~InventoryPage()
	{
		_Search.AddItemClicked -= AddInventory;	
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
    private void AddInventory(object sender, ClickedEventArgs e)
    {
        _viewModel.SelectedInventory = null;
        _viewModel.Clean();
        Navigation.PushAsync(new EditInventoryPage(_languageService, _viewModel));
    }

    private void EditInventory(object sender, EventArgs e)
    {
        if (sender is MaterialArticleCardView card && card.BindingContext is Models.Inventory inventory)
        {
            _viewModel.SelectedInventory = inventory;
            _viewModel.Clean();
            Navigation.PushAsync(new EditInventoryPage(_languageService, _viewModel));
        }
    }
    #endregion
}