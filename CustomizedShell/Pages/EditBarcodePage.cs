using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Maui.Markup;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Enums;
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
    private readonly StyledEntry _NameEntry = new();
    private readonly StyledEntry _DescriptionEntry = new();
    private readonly FloatingActionButton _SaveButton = new();
    private readonly FloatingActionButton _DeleteButton = new();
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

        _NameEntry.Placeholder = _LanguageService.StringForKey("Barcode");
        _NameEntry.Keyboard = Keyboard.Numeric;
        _NameEntry.Text = _Barcode.Name;

        _DescriptionEntry.Placeholder = _LanguageService.StringForKey("Description");
        _DescriptionEntry.Text = _Barcode.Description;

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
        switch (barcodesViewModel.CardStyle)
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

        _ContentLayout.Children.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 64
        });
        
        _SaveButton.FABStyle = FloatingActionButtonStyle.Extended;
        _SaveButton.FABBackgroundColor = Colors.Green;
        _SaveButton.TextColor = Colors.White;
        _DeleteButton.FABStyle = FloatingActionButtonStyle.Extended;
        _DeleteButton.TextColor = Colors.White;
        _DeleteButton.FABBackgroundColor = Colors.Red;

        PopupStyle = PopupStyle.BottomSheet;
        PopupContent = _ContentLayout;

        _ContentLayout.TapGesture(() => 
        {
            if (KeyboardExtensions.IsSoftKeyboardShowing(_NameEntry.TextInput))
            {
                KeyboardExtensions.HideKeyboardAsync(_NameEntry.TextInput);
            }
            else if (KeyboardExtensions.IsSoftKeyboardShowing(_DescriptionEntry.TextInput))
            {
                KeyboardExtensions.HideKeyboardAsync(_DescriptionEntry.TextInput);
            }
        });
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _SaveButton.Clicked += Save;
        _DeleteButton.Clicked += Delete;
        _NameEntry.TextChanged += BarcodeTextChanged;
        _DescriptionEntry.TextChanged += DescriptionChanged;
    }

    protected override void OnDisappearing()
    {
        _SaveButton.Clicked -= Save;
        _DeleteButton.Clicked -= Delete;
        _NameEntry.TextChanged -= BarcodeTextChanged;
        _DescriptionEntry.TextChanged -= DescriptionChanged;
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

        bool wasAbleToDelete = await _BarcodesViewModel.Delete(_Barcode);
        if (!wasAbleToDelete)
        {
            await Application.Current.MainPage.DisplayAlert(
                _LanguageService.StringForKey("ErrorOccurred"),
                _LanguageService.StringForKey("CannotDeleteBarcode") + "\n" + 
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
        if (string.IsNullOrEmpty(_NameEntry.Text))
        {
            _NameEntry.StatusColor = Colors.Red;
            _NameEntry.StatusIcon = "info.png";
            _NameEntry.StatusText = _LanguageService.StringForKey("BarcodeRequired");
        }

        if (string.IsNullOrEmpty(_DescriptionEntry.Text))
        {
            _DescriptionEntry.StatusColor = Colors.Red;
            _DescriptionEntry.StatusIcon = "info.png";
            _DescriptionEntry.StatusText = _LanguageService.StringForKey("DescriptionRequired");
        }

        if (string.IsNullOrEmpty(_NameEntry.Text) || string.IsNullOrEmpty(_DescriptionEntry.Text))
        {
            return;
        }

        bool save = true;

        if (!_IsNew)
        {
            save = await Application.Current.MainPage.DisplayAlert(
                _LanguageService.StringForKey("AreYouSure"),
                _LanguageService.StringForKey("SaveBarcodePrompt"),
                _LanguageService.StringForKey("Save"),
                _LanguageService.StringForKey("Cancel"));
        }

        if (!save)
        {
            return;
        }

        _Barcode.Name = _NameEntry.Text;
        _Barcode.Description = _DescriptionEntry.Text;

        bool wasAbleToSave = await _BarcodesViewModel.Save(_Barcode);
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

    private void BarcodeTextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length > 0)
        {
            _NameEntry.StatusColor = Colors.Black;
        }
        else
        {
            _NameEntry.StatusColor = Colors.Red;
        }
        _NameEntry.StatusIcon = null;
        _NameEntry.StatusText = "";
    }

    private void DescriptionChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length > 0)
        {
            _DescriptionEntry.StatusColor = Colors.Black;
        }
        else
        {
            _DescriptionEntry.StatusColor = Colors.Red;
        }
        _DescriptionEntry.StatusIcon = null;
        _DescriptionEntry.StatusText = "";
    }
    #endregion
}
