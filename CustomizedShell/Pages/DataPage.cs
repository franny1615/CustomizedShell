using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class DataPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _Lang;
    private DataViewModel _DataViewModel => (DataViewModel)BindingContext;
    private readonly VerticalStackLayout _ContentLayout = new()
    {
        Padding = 16,
        Spacing = 16,
    };
    private readonly CardView _Barcodes = new() { ImageSource = "barcode.png" };
    private readonly CardView _Categories = new() { ImageSource = "category.png" };
    private readonly CardView _Statuses = new() { ImageSource = "done.png" };
    #endregion

    #region Constructor
    public DataPage(
        ILanguageService languageService,
        DataViewModel dataViewModel) : base(languageService)
    {
        _Lang = languageService;
        BindingContext = dataViewModel;

        Color primary = Application.Current.Resources["Primary"] as Color;
        Color cardLight = Application.Current.Resources["CardColorLight"] as Color;
        Color cardDark = Application.Current.Resources["CardColorDark"] as Color;
        _Barcodes.SetAppThemeColor(CardView.CardColorProperty, cardLight, cardDark);
        _Categories.SetAppThemeColor(CardView.CardColorProperty, cardLight, cardDark);
        _Statuses.SetAppThemeColor(CardView.CardColorProperty, cardLight, cardDark);
        _Barcodes.ImageBackgroundColor = primary;
        _Categories.ImageBackgroundColor = primary;
        _Statuses.ImageBackgroundColor = primary;

        _Barcodes.Title = _Lang.StringForKey("Barcodes");
        _Categories.Title = _Lang.StringForKey("Categories");
        _Statuses.Title = _Lang.StringForKey("Statuses");

        _ContentLayout.Children.Add(_Barcodes);
        _ContentLayout.Children.Add(_Categories);
        _ContentLayout.Children.Add(_Statuses);

        Content = _ContentLayout;
    }
    #endregion

    #region Override
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _Barcodes.Clicked += GoToBarcodes;
        _Categories.Clicked += GoToCategories;
        _Statuses.Clicked += GoToStatuses;
        FetchCounts();
    }

    protected override void OnDisappearing()
    {
        _Barcodes.Clicked -= GoToBarcodes;
        _Categories.Clicked -= GoToCategories;
        _Statuses.Clicked -= GoToStatuses;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void FetchCounts()
    {
        string barcodesTemplate = _Lang.StringForKey("BarcodeCount");
        string categoriesTemplate = _Lang.StringForKey("CategoryCount");
        string statusesTemplate = _Lang.StringForKey("StatusCount");

        int barcodeCount = await _DataViewModel.GetBarcodeCount();
        int categoryCount = await _DataViewModel.GetCategoryCount();
        int statusCount = await _DataViewModel.GetStatusesCount();

        _Barcodes.Description = string.Format(barcodesTemplate, barcodeCount);
        _Categories.Description = string.Format(categoriesTemplate, categoryCount);
        _Statuses.Description = string.Format(statusesTemplate, statusCount);
    }
    
    private async void GoToStatuses(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StatusesPage));
    }

    private async void GoToCategories(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CategoriesPage));
    }

    private void GoToBarcodes(object sender, EventArgs e)
    {
        // TODO:
    }
    #endregion
}
