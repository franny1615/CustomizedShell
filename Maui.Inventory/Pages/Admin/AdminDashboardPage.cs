using Maui.Components;
using Maui.Components.Pages;

namespace Maui.Inventory.Pages.Admin;

public class AdminDashboardPage : BasePage
{
    public AdminDashboardPage(ILanguageService languageService) : base(languageService)
    {
        Content = new VerticalStackLayout
        {
            Children = 
            {
                new Label 
                {
                    FontSize = 21,
                    FontAttributes = FontAttributes.Bold,
                    Text = "Dashboard"
                }
            }
        };
    }
}