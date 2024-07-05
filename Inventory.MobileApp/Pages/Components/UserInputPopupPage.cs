using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Services;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Pages.Components;

public class UserInputPopupPage : PopupPage
{
    private readonly Grid _ContentLayout = new Grid
    {
        RowDefinitions = Rows.Define(Star, Auto),
        RowSpacing = 12,
        Padding = 12,
    }; 
    private readonly MaterialEntry _UserInput = new MaterialEntry
    {
        Placeholder = "",
        IsMultiLine = true,
    };
    private readonly Button _Cancel = new Button()
    {
        Text = LanguageService.Instance["Cancel"],
        BackgroundColor = Color.FromArgb("#646464"),
        TextColor = Colors.White,
    };
    private readonly Button _Submit = new Button()
    {
        Text = LanguageService.Instance["OK"],
    };

    private Action<string> _Submitted;
    private Action _Canceled;

    public UserInputPopupPage(
        string title,
        string existingText,
        Keyboard keyboard,
        Action<string> submitted,
        Action canceled)
    {
        _UserInput.Placeholder = title;
        _Submitted = submitted;
        _Canceled = canceled;
        _UserInput.Keyboard = keyboard;
        _UserInput.Text = existingText;

        _ContentLayout.SetDynamicResource(BackgroundProperty, "DashTileColor");
        _ContentLayout.Add(_UserInput.Row(0));
        _ContentLayout.Add(new Grid
        {
            ColumnDefinitions = Columns.Define(Auto, Star),
            ColumnSpacing = 12,
            Children =
            {
                _Cancel,
                _Submit.Column(1),
            }
        }.Row(1));

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;

        _Cancel.Clicked += Cancel;
        _Submit.Clicked += Submit;
    }

    private void Submit(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
        _Submitted?.Invoke(_UserInput.Text);
    }

    private void Cancel(object? sender, EventArgs e)
    {
        Navigation.PopModalAsync();
        _Canceled?.Invoke();
    }
}
