using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class CategoriesPage : SearchPage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly CategoriesViewModel _CategoriesViewModel;
    #endregion

    #region Constructor
    public CategoriesPage(
        ILanguageService languageService,
        CategoriesViewModel categoriesViewModel) : base(languageService, categoriesViewModel)
    {
        _LanguageService = languageService;
        _CategoriesViewModel = categoriesViewModel;
        OverrideBackButtonText();
    }
    #endregion

    #region Override
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Add += AddCategory;
        Edit += EditCategory;
    }

    protected override void OnDisappearing()
    {
        Add -= AddCategory;
        Edit -= EditCategory;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void AddCategory(object sender, EditEventArgs e)
    {
        await this.Navigation.PushModalAsync(new EditCategoryPage(
            _LanguageService,
            _CategoriesViewModel,
            e.Item as Category,
            isNew: true
        ));
    }

    private async void EditCategory(object sender, EditEventArgs e)
    {
        await this.Navigation.PushModalAsync(new EditCategoryPage(
            _LanguageService,
            _CategoriesViewModel,
            e.Item as Category
        ));
    }
    #endregion
}
