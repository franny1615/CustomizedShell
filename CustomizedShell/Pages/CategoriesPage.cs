using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class CategoriesPage : SearchPage
{
    public CategoriesPage(
        ILanguageService languageService,
        CategoriesViewModel categoriesViewModel) : base(languageService, categoriesViewModel)
    {
        OverrideBackButtonText();
    }
}
