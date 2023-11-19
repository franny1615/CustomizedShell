using CustomizedShell.ViewModels;
using Maui.Components.Controls;

namespace CustomizedShell.Pages;

public class DataPage : BasePage
{
    #region Private Properties
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
    public DataPage(DataViewModel dataViewModel)
    {
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

        _Barcodes.Title = Lang["Barcodes"];
        _Categories.Title = Lang["Categories"];
        _Statuses.Title = Lang["Statuses"];

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
        string barcodesTemplate = Lang["BarcodeCount"];
        string categoriesTemplate = Lang["CategoryCount"];
        string statusesTemplate = Lang["StatusCount"];

        int barcodeCount = await _DataViewModel.GetBarcodeCount();
        int categoryCount = await _DataViewModel.GetCategoryCount();
        int statusCount = await _DataViewModel.GetStatusesCount();

        _Barcodes.Description = string.Format(barcodesTemplate, barcodeCount);
        _Categories.Description = string.Format(categoriesTemplate, categoryCount);
        _Statuses.Description = string.Format(statusesTemplate, statusCount);
    }
    
    private void GoToStatuses(object sender, EventArgs e)
    {
        // TODO:
    }

    private void GoToCategories(object sender, EventArgs e)
    {
        // TODO:
    }

    private void GoToBarcodes(object sender, EventArgs e)
    {
        // TODO:
    }
    #endregion
}
