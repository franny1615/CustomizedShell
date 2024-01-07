using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Inventory;

public class AdminUpdateEmailPage : PopupPage
{
    #region Private Properties
    private readonly ILanguageService _LangService;
    private readonly ScrollView _ContentScroll = new();
    private readonly MaterialImage _CloseIcon = new()
    {
        Icon = MaterialIcon.Close,
        IconSize = 25,
        IconColor = Colors.DarkGray,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly Label _Title = new()
    {
        FontSize = 18,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Center,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(30, Star, Auto),
        ColumnDefinitions = Columns.Define(30, Star, 30),
        Margin = new Thickness(0, 0, 0, 32),
        ColumnSpacing = 8,
        RowSpacing = 12,
    };
    private readonly VerticalStackLayout _EntryContainer = new()
    {
        Spacing = 12
    };
    private readonly MaterialEntry _Email;
    private readonly MaterialEntry _VerificationCode;
    private readonly MaterialEntry _Password;
    #endregion

    #region Constructor
    public AdminUpdateEmailPage(ILanguageService languageService) : base(languageService)
    {
        _LangService = languageService;

        _ContentScroll.Content = _ContentLayout;

        _Title.Text = _LangService.StringForKey("UpdateEmail");

        _CloseIcon.TapGesture(async () => 
        {
            await _CloseIcon.ScaleTo(0.95, 70);
            await _CloseIcon.ScaleTo(1.0, 70);

            await Navigation.PopModalAsync();
        });

        _ContentLayout.Children.Add(_CloseIcon.Row(0).Column(2));
        _ContentLayout.Children.Add(_Title.Row(0).Column(1));
        _ContentLayout.Children.Add(_EntryContainer.Row(1).Column(0).ColumnSpan(3));

        PopupStyle = PopupStyle.BottomSheet;
        PopupContent = _ContentScroll;
    }
    #endregion
}
