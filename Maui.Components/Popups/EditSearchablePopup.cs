using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Maui.Components.Controls;
using Maui.Components.Enums;
using Maui.Components.Interfaces;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Maui.Components.Popups;

public class EditSearchablePopup : Popup
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private readonly ISearchViewModel _SearchViewModel;
    private readonly ISearchable _Searchable;
    private readonly VerticalStackLayout _ContentLayout = new()
    {
        Spacing = 8,
        Padding = 16
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
    private readonly StyledEntry _NameEntry = new();
    private readonly StyledEntry _DescriptionEntry = new();
    private readonly FloatingActionButton _SaveButton = new();
    private readonly FloatingActionButton _DeleteButton = new();
    private readonly IconTintColorBehavior _CloseIconTint = new();
    #endregion

    #region Constructor
    public EditSearchablePopup(
        ILanguageService languageService,
        ISearchable searchable,
        ISearchViewModel searchViewModel,
        EditSearchableArgs args,
        CardStyle cardStyle,
        bool isNew = false)
    {
        _LanguageService = languageService;
        _Searchable = searchable;
        _SearchViewModel = searchViewModel;

        this.SetAppThemeColor(
            Popup.ColorProperty, 
            Colors.White,
            Application.Current.Resources["CardColorDark"] as Color);
        _CloseIconTint.SetAppThemeColor(IconTintColorBehavior.TintColorProperty, Colors.Black, Colors.White);
        _CloseIcon.Behaviors.Add(_CloseIconTint);

        _CloseIcon.TapGesture(async () => 
        {
            await this.CloseAsync();
        });

        _CloseIcon.Source = args.CloseIcon;
        _TitleLabel.Text = args.Title;

        _NameEntry.Placeholder = args.NamePlaceholder;
        _DescriptionEntry.Placeholder = args.DescriptionPlaceholder;

        _NameEntry.Text = searchable.Name;
        _DescriptionEntry.Text = searchable.Description;

        _SaveButton.Text = args.SavePlaceholder;
        _SaveButton.ImageSource = args.SaveIcon;

        _DeleteButton.Text = args.DeletePlaceholder;
        _DeleteButton.ImageSource = args.DeleteIcon;

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
			HeightRequest = 16
		});
        switch (cardStyle)
        {
            case CardStyle.Mini:
                _ContentLayout.Children.Add(_NameEntry);
                break;
            case CardStyle.Regular:
                _ContentLayout.Children.Add(_NameEntry);
                _ContentLayout.Children.Add(_DescriptionEntry);
                break;
        }

        _ContentLayout.Children.Add(new BoxView
		{
			Color = Colors.Transparent,
			HeightRequest = 16
		});
        _ContentLayout.Children.Add(_SaveButton);
        
        if (!isNew)
        {
            _ContentLayout.Children.Add(new BoxView
            {
                Color = Colors.Transparent,
                HeightRequest = 8
            }); 
            _ContentLayout.Children.Add(_DeleteButton);
        }
        
        _SaveButton.FABStyle = FloatingActionButtonStyle.Extended;
        _SaveButton.FABBackgroundColor = Colors.Green;
        _SaveButton.TextColor = Colors.White;
        _DeleteButton.FABStyle = FloatingActionButtonStyle.Extended;
        _DeleteButton.TextColor = Colors.White;
        _DeleteButton.FABBackgroundColor = Colors.Red;

        Content = _ContentLayout;

        ApplyContentWidth(DeviceDisplay.Current.MainDisplayInfo);

        Opened += HasOpened;
        Closed += HasClosed;
    }
    #endregion

    #region Helpers
    private void HasOpened(object sender, PopupOpenedEventArgs e)
    {
        DeviceDisplay.Current.MainDisplayInfoChanged += NewDisplayInfo;
    }

    private void HasClosed(object sender, PopupClosedEventArgs e)
    {
        DeviceDisplay.Current.MainDisplayInfoChanged -= NewDisplayInfo;
    }

    private void NewDisplayInfo(object sender, DisplayInfoChangedEventArgs e)
    {
        ApplyContentWidth(e.DisplayInfo);
    }

    private void ApplyContentWidth(DisplayInfo info)
    {
        _ContentLayout.WidthRequest = info.Width / info.Density * 0.8;
    }
    #endregion
}
