using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class StatusesPage : SearchPage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    #endregion

    #region Constructor
    public StatusesPage(
        ILanguageService languageService,
        StatusesViewModel statusViewModel) : base(languageService, statusViewModel)
    {
        _LanguageService = languageService;
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
        await this.Navigation.PushModalAsync(new PopupPage(_LanguageService)
        {
            PopupStyle = PopupStyle.BottomSheet
        });
    }

    private void EditStatus(object sender, EditEventArgs e)
    {
        
    }
    #endregion
}
