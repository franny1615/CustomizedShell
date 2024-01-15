using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminUsersPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private readonly Grid _ContentLayout = new()
    {
        Padding = 16
    };
    private readonly FloatingActionButton _AddUser = new()
    {
        ImageSource = UIUtils.MaterialIconFIS(MaterialIcon.Add, Colors.White),
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        FABStyle = FloatingActionButtonStyle.Regular,
        ZIndex = 1,
    };
    #endregion

    #region Constructor
    public AdminUsersPage(
        ILanguageService languageService) : base(languageService)
    {
        _LangService = languageService;

        Title = _LangService.StringForKey("Employees");

        _ContentLayout.Children.Add(_AddUser.End().Bottom());

        Content = _ContentLayout;
    }
    #endregion
}