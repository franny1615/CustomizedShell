using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class BarcodesPage : SearchPage
{
    public BarcodesPage(
        ILanguageService languageService, 
        BarcodesViewModel barcodesViewModel) : base(languageService, barcodesViewModel)
    {
        OverrideBackButtonText();
    }
}
