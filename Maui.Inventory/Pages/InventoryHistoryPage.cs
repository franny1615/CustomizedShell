using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory.Pages;

public class InventoryHistoryPage : PopupPage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private InventoryHistoryViewModel _ViewModel => (InventoryHistoryViewModel) BindingContext;
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(50, Star),
        ColumnDefinitions = Columns.Define(30, Star, 30),
        ColumnSpacing = 0,
        RowSpacing = 0,
        Padding = 0
    };
    private readonly Label _Title = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
    };
    private readonly MaterialImage _Close = new()
    {
        Icon = MaterialIcon.Close,
        IconSize = 30,
        IconColor = Application.Current.Resources["TextColor"] as Color
    };
    private readonly MaterialList<Models.Inventory> _Search;
    #endregion

    #region Constructor
    public InventoryHistoryPage(
        ILanguageService languageService,
        InventoryHistoryViewModel viewModel) : base(languageService)
    {
        _LangService = languageService;
        BindingContext = viewModel;

        _Close.TapGesture(() => Navigation.PopModalAsync());

        _ContentLayout.Children.Add(_Title.Row(0).Column(1).Center());
        _ContentLayout.Children.Add(_Close.Row(0).Column(2).Center());

        _Title.Text = _LangService.StringForKey("History");
        _Search = new(_LangService.StringForKey("No Inventory History."), MaterialIcon.Inventory_2, new DataTemplate(() =>
        {
            var view = new MaterialArticleCardView();
            view.SetBinding(MaterialArticleCardView.BindingContextProperty, ".");
            view.SetBinding(MaterialArticleCardView.ArticleProperty, "Description");
            view.SetBinding(MaterialArticleCardView.MainSupportOneProperty, "Location");
            view.SetBinding(MaterialArticleCardView.MainSupportTwoProperty, "QuantityStr");
            view.SetBinding(MaterialArticleCardView.TitleProperty, "Status");
            view.SetDynamicResource(MaterialCardView.BackgroundColorProperty, "CardColor");
            view.TextColor = Colors.White;

            return view;
        }), viewModel, isEditable: false);

        _ContentLayout.Children.Add(_Search.Row(1).Column(0).ColumnSpan(3));

        PopupStyle = PopupStyle.BottomSheet;
        PopupContent = _ContentLayout;
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
}
