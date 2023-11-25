using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class EditBarcodePage : PopupPage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly BarcodesViewModel _BarcodesViewModel;
    private readonly Barcode _Barcode;
    private bool _IsNew;
    private readonly IconTintColorBehavior _CloseIconTint = new();
    private readonly VerticalStackLayout _ContentLayout = new()
    {
        Spacing = 8,
    };
    private readonly Label _TitleLabel = new()
    {
        FontSize = 21,
        FontAttributes = FontAttributes.Bold,
        HorizontalTextAlignment = TextAlignment.Start,
        VerticalOptions = LayoutOptions.Center,
        MaxLines = 1
    };
    private readonly Image _CloseIcon = new()
    {
        HeightRequest = 30,
        WidthRequest = 30,
    };
    #endregion

    #region Constructor
    public EditBarcodePage(
        ILanguageService languageService,
        BarcodesViewModel barcodesViewModel,
        Barcode barcode,
        bool isNew = false) : base(languageService)
    {
        _LanguageService = languageService;
        _BarcodesViewModel = barcodesViewModel;
        _Barcode = barcode;
        _IsNew = isNew;
        
        _CloseIconTint.SetAppThemeColor(IconTintColorBehavior.TintColorProperty, Colors.Black, Colors.White);
        _CloseIcon.Behaviors.Add(_CloseIconTint);

        _CloseIcon.TapGesture(async () => 
        {
            await this.Navigation.PopModalAsync();
        });

        _CloseIcon.Source = "close.png";
        _TitleLabel.Text = isNew ? _LanguageService.StringForKey("AddBarcode") : _LanguageService.StringForKey("EditBarcode");

        _ContentLayout.Children.Add(new Grid 
        {
            Children = 
            {
                _TitleLabel.Start(),
                _CloseIcon.End()
            }
        });
        _ContentLayout.Children.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 64
        });

        PopupStyle = PopupStyle.BottomSheet;
        PopupContent = _ContentLayout;
    }
    #endregion
}
