using Maui.Inventory.Models;
using Maui.Inventory.ViewModels;
using Maui.Components;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages;

public class StatusesPage : SearchPage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly StatusesViewModel _StatusViewModel;
    #endregion

    #region Constructor
    public StatusesPage(
        ILanguageService languageService,
        StatusesViewModel statusViewModel) : base(languageService, statusViewModel)
    {
        _LanguageService = languageService;
        _StatusViewModel = statusViewModel;
        OverrideBackButtonText();
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Add += AddStatus;
        Edit += EditStatus;
    }

    protected override void OnDisappearing()
    {
        Add -= AddStatus;
        Edit -= EditStatus;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void AddStatus(object sender, EditEventArgs e)
    {
        await this.Navigation.PushModalAsync(new EditStatusPage(
            _LanguageService,
            _StatusViewModel,
            e.Item as Status,
            isNew: true));
    }

    private async void EditStatus(object sender, EditEventArgs e)
    {
        await this.Navigation.PushModalAsync(new EditStatusPage(
            _LanguageService,
            _StatusViewModel,
            e.Item as Status));
    }
    #endregion
}
