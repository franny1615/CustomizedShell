using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory;

public static class UIUtils
{
    public static Grid HorizontalRuleWithText(string text)
    {
        return new Grid 
        {
            ColumnDefinitions = Columns.Define(Star, 50, Star),
            ColumnSpacing = 8,
            Children = 
            {
                new BoxView
                {
                    HeightRequest = 1,
                    Color = Application.Current.Resources["Primary"] as Color
                }.Column(0),
                new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    Text = text
                }.Column(1),
                new BoxView
                {
                    HeightRequest = 1,
                    Color = Application.Current.Resources["Primary"] as Color
                }.Column(2)
            }
        };
    }
}
