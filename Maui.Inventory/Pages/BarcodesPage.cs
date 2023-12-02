using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Components;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages;

public class BarcodesPage : SearchPage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly BarcodesViewModel _BarcodesViewModel;
    #endregion

    #region Constructor
    public BarcodesPage(
        ILanguageService languageService, 
        BarcodesViewModel barcodesViewModel) : base(languageService, barcodesViewModel)
    {
        OverrideBackButtonText();
        _LanguageService = languageService;
        _BarcodesViewModel = barcodesViewModel;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Add += AddBarcode;
        Edit += EditBarcode;
    }

    protected override void OnDisappearing()
    {
        Add -= AddBarcode;
        Edit -= EditBarcode;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void AddBarcode(object sender, EditEventArgs e)
    {
        await this.Navigation.PushModalAsync(new EditBarcodePage(
            _LanguageService,
            _BarcodesViewModel,
            e.Item as Barcode,
            isNew: true
        ));
    }

    private async void EditBarcode(object sender, EditEventArgs e)
    {
        await this.Navigation.PushModalAsync(new EditBarcodePage(
            _LanguageService,
            _BarcodesViewModel,
            e.Item as Barcode
        ));
    }
    #endregion
}
