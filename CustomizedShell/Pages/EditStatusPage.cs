using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Enums;
using Maui.Components.Pages;

namespace CustomizedShell.Pages;

public class EditStatusPage : PopupPage
{
    #region Private Properties
    private readonly StatusesViewModel _StatusesViewModel;
    private readonly ILanguageService _LanguageService;
    private bool _IsNew;
    private readonly Status _Status;
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
    private readonly StyledEntry _NameEntry = new();
    private readonly StyledEntry _DescriptionEntry = new();
    private readonly FloatingActionButton _SaveButton = new();
    private readonly FloatingActionButton _DeleteButton = new();
    private readonly IconTintColorBehavior _CloseIconTint = new();
    #endregion

    #region Constructor
    public EditStatusPage(        
        ILanguageService languageService,
        StatusesViewModel statusesViewModel,
        Status status,
        bool isNew = false) : base(languageService)
    {
        _Status = status;
        _StatusesViewModel = statusesViewModel;
        _LanguageService = languageService;
        _IsNew = isNew;

        _CloseIconTint.SetAppThemeColor(IconTintColorBehavior.TintColorProperty, Colors.Black, Colors.White);
        _CloseIcon.Behaviors.Add(_CloseIconTint);

        _CloseIcon.TapGesture(async () => 
        {
            await this.Navigation.PopModalAsync();
        });

        _CloseIcon.Source = "close.png";
        _TitleLabel.Text = isNew ? _LanguageService.StringForKey("AddStatus") : _LanguageService.StringForKey("EditStatus");

        _NameEntry.Placeholder = _LanguageService.StringForKey("Status");
        _DescriptionEntry.Placeholder = _LanguageService.StringForKey("Description");

        _NameEntry.Text = _Status.Name;
        _DescriptionEntry.Text = _Status.Description;

        _SaveButton.Text = _LanguageService.StringForKey("Save");
        _SaveButton.ImageSource = "add.png";

        _DeleteButton.Text = _LanguageService.StringForKey("Delete");
        _DeleteButton.ImageSource = "trash.png";

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
        switch (statusesViewModel.CardStyle)
        {
            case CardStyle.Mini:
                _ContentLayout.Children.Add(_NameEntry);
                break;
            case CardStyle.Regular:
                _ContentLayout.Children.Add(_NameEntry);
                _ContentLayout.Children.Add(new BoxView
                {
                    Color = Colors.Transparent,
                    HeightRequest = 8
                });
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

        PopupStyle = PopupStyle.Center;
        PopupContent = _ContentLayout;
    }
    #endregion 

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _SaveButton.Clicked += Save;
        _DeleteButton.Clicked += Delete;
    }

    protected override void OnDisappearing()
    {
        _SaveButton.Clicked -= Save;
        _DeleteButton.Clicked -= Delete;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async void Delete(object sender, ClickedEventArgs e)
    {
        bool delete = await Application.Current.MainPage.DisplayAlert(
            _LanguageService.StringForKey("AreYouSure"),
            _LanguageService.StringForKey("DeletePrompt"),
            _LanguageService.StringForKey("Yes"),
            _LanguageService.StringForKey("No"));

        if (!delete)
        {
            return;
        }

        bool wasAbleToDelete = await _StatusesViewModel.Delete(_Status);
        if (!wasAbleToDelete)
        {
            await Application.Current.MainPage.DisplayAlert(
                _LanguageService.StringForKey("ErrorOccurred"),
                _LanguageService.StringForKey("CannotDeleteStatus") + "\n" + 
                    _LanguageService.StringForKey("Or") + "\n" + 
                    _LanguageService.StringForKey("ErrorMessage"),
                _LanguageService.StringForKey("Ok"));
        }
        else
        {
            await this.Navigation.PopModalAsync();
        }
    }

    private async void Save(object sender, ClickedEventArgs e)
    {
        bool save = true;

        if (!_IsNew)
        {
            save = await Application.Current.MainPage.DisplayAlert(
                _LanguageService.StringForKey("AreYouSure"),
                _LanguageService.StringForKey("SaveStatusPrompt"),
                _LanguageService.StringForKey("Save"),
                _LanguageService.StringForKey("Cancel"));
        }

        if (!save)
        {
            return;
        }

        _Status.Name = _NameEntry.Text;
        _Status.Description = _DescriptionEntry.Text;

        bool wasAbleToSave = await _StatusesViewModel.Save(_Status);
        if (!wasAbleToSave)
        {
            await Application.Current.MainPage.DisplayAlert(
                _LanguageService.StringForKey("ErrorOccurred"),
                _LanguageService.StringForKey("ErrorMessage"),
                _LanguageService.StringForKey("Ok"));
        }
        else
        {
            await this.Navigation.PopModalAsync();
        }
    }
    #endregion
}
