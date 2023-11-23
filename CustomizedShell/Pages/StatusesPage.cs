using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class StatusesPage : SearchPage
{
    public StatusesPage(
        ILanguageService languageService,
        StatusesViewModel statusViewModel) : base(languageService, statusViewModel)
    {
        OverrideBackButtonText();
    }
}
