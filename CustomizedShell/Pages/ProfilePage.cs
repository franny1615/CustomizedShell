using CommunityToolkit.Mvvm.Messaging;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Components.Utilities;
using Microsoft.Maui.Controls.Shapes;
using System.Runtime.CompilerServices;

namespace CustomizedShell.Pages;

public class ProfilePage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _LanguageService;
    private ProfileViewModel _ProfileViewModel => (ProfileViewModel) BindingContext;
    private readonly ScrollView _ContentScroll = new();
	private readonly VerticalStackLayout _ContentLayout = new()
	{
		Padding = 16
	};
    private readonly StyledEntry _Username = new();
	private readonly StyledEntry _Password = new() { IsPassword = true };
    private readonly StyledEntry _Email = new() { Keyboard = Keyboard.Email };
    private readonly FloatingActionButton _Save = new()
	{
        TextColor = Colors.White,
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        FABStyle = FloatingActionButtonStyle.Extended
	};
    private readonly FloatingActionButton _DeleteAccount = new()
	{
        TextColor = Colors.White,
        FABBackgroundColor = Application.Current.Resources["Negative"] as Color,
        FABStyle = FloatingActionButtonStyle.Extended
	};
    #endregion

    #region Constructors
    public ProfilePage(
        ILanguageService languageService, 
        ProfileViewModel profileViewModel) : base (languageService)
    {
        _LanguageService = languageService;
        BindingContext = profileViewModel;

        _Username.Placeholder = languageService.StringForKey("Username");
		_Password.Placeholder = languageService.StringForKey("Password");
        _Email.Placeholder = languageService.StringForKey("Email");
        _Save.Text = languageService.StringForKey("Save");
        _DeleteAccount.Text = languageService.StringForKey("DeleteAccount");

        _ContentLayout.Add(new Border
		{
			Stroke = Colors.Transparent,
			StrokeShape = new RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Application.Current.Resources["Primary"] as Color,
			HorizontalOptions = LayoutOptions.Center,
            Padding = 0,
			Margin = 0,
			Content = new Image
			{
				WidthRequest = 124,
				HeightRequest = 124,
				Source = "app_ic.png",
				HorizontalOptions = LayoutOptions.Center,
			}
		});
        _ContentLayout.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 16
        });
        _ContentLayout.Add(new Label
		{
            Text = languageService.StringForKey("Profile"),
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = 8
        });
        _ContentLayout.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 16
        });
        _ContentLayout.Add(_Username);
        _ContentLayout.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 8
        });
        _ContentLayout.Add(_Password);
        _ContentLayout.Add(new BoxView
        {
            Color = Colors.Transparent,
            HeightRequest = 8
        });
        _ContentLayout.Add(_Email);
		_ContentLayout.Add(new BoxView
		{
			Color = Colors.Transparent,
			HeightRequest = 64
		});
        _ContentLayout.Add(_Save);
        _ContentLayout.Add(new BoxView
		{
			Color = Colors.Transparent,
			HeightRequest = 16
		});
        _ContentLayout.Add(_DeleteAccount);

        _ContentScroll.Content = _ContentLayout;
        Content = _ContentScroll;
    }
    #endregion

    #region Overrides
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await FetchUser();
        _Save.Clicked += Save;
        _DeleteAccount.Clicked += DeleteAccount;
    }

    protected override void OnDisappearing()
    {
        _Save.Clicked -= Save;
        _DeleteAccount.Clicked -= DeleteAccount;
        base.OnDisappearing();
    }
    #endregion

    #region Helpers
    private async Task FetchUser()
    {
        var user = await _ProfileViewModel.GetLoggedInUser();
        if (user == null) 
        {
            return;
        }

        _Username.Text = user.Username;
        _Password.Text = user.Password;
        _Email.Text = user.Email;

        _Username.StatusColor = Colors.Black;
        _Username.StatusIcon = null;
        _Username.StatusText = "";

        _Password.StatusColor = Colors.Black;
        _Password.StatusIcon = null;
        _Password.StatusText = "";

        _Email.StatusColor = Colors.Black;
        _Email.StatusIcon = null;
        _Email.StatusText = "";
    }

    private async void Save(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_Username.Text))
        {
            _Username.StatusColor = Colors.Red;
            _Username.StatusIcon = "info.png";
            _Username.StatusText = _LanguageService.StringForKey("UsernameRequired");
        }

        if (string.IsNullOrEmpty(_Password.Text))
        {
            _Password.StatusColor = Colors.Red;
            _Password.StatusIcon = "info.png";
            _Password.StatusText = _LanguageService.StringForKey("PasswordRequired");
        }

        if (string.IsNullOrEmpty(_Email.Text))
        {
            _Email.StatusColor = Colors.Red;
            _Email.StatusIcon = "info.png";
            _Email.StatusText = _LanguageService.StringForKey("EmailRequired");
        }
        else if (!StringUtils.IsValidEmail(_Email.Text))
        {
            _Email.StatusColor = Colors.Red;
            _Email.StatusIcon = "info.png";
            _Email.StatusText = _LanguageService.StringForKey("EmailInvalid");
        }

        if (string.IsNullOrEmpty(_Email.Text) || 
            string.IsNullOrEmpty(_Password.Text) || 
            string.IsNullOrEmpty(_Username.Text) ||
            !StringUtils.IsValidEmail(_Email.Text))
        {
            return;    
        }

        await _ProfileViewModel.Save(
            _Username.Text,
            _Password.Text,
            _Email.Text
        );

        await FetchUser();

        await DisplayAlert(
            _LanguageService.StringForKey("Profile"),
            _LanguageService.StringForKey("ChangesSaved"),
            _LanguageService.StringForKey("Ok")
        );
    }

    private async void DeleteAccount(object sender, EventArgs e)
    {
        bool shouldDelete = await DisplayAlert(
            _LanguageService.StringForKey("AreYouSure"),
            _LanguageService.StringForKey("ProfileDeletePrompt"),
            _LanguageService.StringForKey("Yes"),
            _LanguageService.StringForKey("No")
        );

        if (shouldDelete)
        {
            await _ProfileViewModel.DeleteAccount();
            WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-out"));
        }
    }
    #endregion
}
