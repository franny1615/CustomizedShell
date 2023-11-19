using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using CoreBluetooth;
using CustomizedShell.Models;
using CustomizedShell.Services;
using CustomizedShell.ViewModels;
using Maui.Components.Controls;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace CustomizedShell.Views;

public enum StatusDetailsMode
{
    Edit,
    New 
}

public class StatusDetailsPopupView : Popup
{
    #region Private Properties
    private readonly Color _Primary = Application.Current.Resources["Primary"] as Color;
    private readonly Color _CardLight = Application.Current.Resources["CardColorLight"] as Color;
    private readonly Color _CardDark = Application.Current.Resources["CardColorDark"] as Color;
    private DataViewModel _DataViewModel => (DataViewModel)BindingContext;
    private Status _Status;
    private StatusDetailsMode _Mode;
    private readonly Grid _ContentLayout = new()
    {
        RowDefinitions = Rows.Define(60, Star, Auto),
        ColumnDefinitions = Columns.Define(Star, Star, Star),
        RowSpacing = 8,
        Padding = 16
    };
    private readonly Label _Header = new()
    {
        FontSize = 16,
        FontAttributes = FontAttributes.Bold,
        VerticalOptions = LayoutOptions.Center
    };
    private readonly FloatingActionButton _Close = new()
    {
        ImageSource = "close.png",
        FABBackgroundColor = Application.Current.Resources["Negative"] as Color,
        FABStyle = FloatingActionButtonStyle.Small
    };
    private readonly StyledEntry _StatusNameEntry = new()
    {
        Placeholder = LanguageService.Instance["Status"]
    };
    private readonly FloatingActionButton _Save = new()
    {
        Text = LanguageService.Instance["Save"],
        TextColor = Colors.White,
        FABBackgroundColor = Application.Current.Resources["Positive"] as Color,
        FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly FloatingActionButton _Delete = new()
    {
        Text = LanguageService.Instance["Delete"],
        TextColor = Colors.White,
        FABBackgroundColor = Application.Current.Resources["Negative"] as Color,
        FABStyle = FloatingActionButtonStyle.Extended
    };
    #endregion

    #region Constructor
    public StatusDetailsPopupView(
        Status status, 
        StatusDetailsMode mode,
        DataViewModel dataViewModel)
    {
        this.SetAppThemeColor(ColorProperty, _CardLight, _CardDark);
        CanBeDismissedByTappingOutsideOfPopup = false;

        _Status = status;
        _Mode = mode;
        BindingContext = dataViewModel;
        
        var screen = DeviceDisplay.Current.MainDisplayInfo;
        _ContentLayout.WidthRequest = (screen.Width / screen.Density) * 0.8;
        _ContentLayout.Children.Add(_Header.Column(1).Center());
        _ContentLayout.Children.Add(_Close.Column(2).End().CenterVertical());
        
        _ContentLayout.Children.Add(_StatusNameEntry.Row(1).Column(0).ColumnSpan(3).CenterVertical());

        switch(mode)
        {
            case StatusDetailsMode.New:
                _Header.Text = LanguageService.Instance["NewStatus"];
                _ContentLayout.Children.Add(_Save.Row(2).Column(0).ColumnSpan(3));
                break;
            case StatusDetailsMode.Edit:
                _Header.Text = LanguageService.Instance["EditStatus"];
                _ContentLayout.Children.Add(new VerticalStackLayout
                {
                    Spacing = 16,
                    Children = 
                    {
                        _Save, 
                        _Delete
                    }
                }.Row(2).Column(0).ColumnSpan(3));
                break;
        }

        Content = _ContentLayout;

        Opened += HasOpened;
        Closed += HasClosed;
    }
    #endregion

    #region Helpers
    private void HasOpened(object sender, PopupOpenedEventArgs e)
    {
        _Close.Clicked += ClosePopup;
        _Save.Clicked += Save;
        _Delete.Clicked += Delete;
    }

    private void HasClosed(object sender, PopupClosedEventArgs e)
    {
        _Close.Clicked -= ClosePopup;
        _Save.Clicked -= Save;
        _Delete.Clicked -= Delete;
    }

    private async void Save(object sender, ClickedEventArgs e)
    {
        if (string.IsNullOrEmpty(_StatusNameEntry.Text))
        {
            await Shell.Current.CurrentPage.DisplayAlert(
                LanguageService.Instance["Save"],
                LanguageService.Instance["CannotSaveEmptyStatus"],
                LanguageService.Instance["Ok"]
            );
            return;
        }

        bool canSave = true;
        if (_Mode == StatusDetailsMode.Edit)
        {
            canSave = await Shell.Current.CurrentPage.DisplayAlert(
                LanguageService.Instance["Save"],
                LanguageService.Instance["SaveStatusPrompt"],
                LanguageService.Instance["Yes"],
                LanguageService.Instance["No"]
            );
        }

        if (canSave)
        {
            _Status.Name = _StatusNameEntry.Text;
            await _DataViewModel.SaveStatus(_Status);
            ClosePopup(this, null);
        }
    }

    private async void Delete(object sender, ClickedEventArgs e)
    {
        bool canDelete = await _DataViewModel.CanDeleteStatus(_Status);
        if (canDelete)
        {
            await _DataViewModel.DeleteStatus(_Status);
            ClosePopup(this, null);
        }
        else
        {
            await Shell.Current.CurrentPage.DisplayAlert(
                LanguageService.Instance["Delete"],
                LanguageService.Instance["CannotDeleteStatus"],
                LanguageService.Instance["Ok"]
            );
        }
    }

    private async void ClosePopup(object sender, ClickedEventArgs e)
    {
        await this.CloseAsync();
    }
    #endregion
}
