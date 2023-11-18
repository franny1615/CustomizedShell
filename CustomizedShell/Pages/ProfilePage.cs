using CommunityToolkit.Mvvm.Messaging;
using CustomizedShell.Models;
using CustomizedShell.ViewModels;
using Maui.Components.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace CustomizedShell.Pages;

public class ProfilePage : BasePage
{
    #region Private Properties
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
    public ProfilePage(ProfileViewModel profileViewModel)
    {
        BindingContext = profileViewModel;

        _Username.Placeholder = Lang["Username"];
		_Password.Placeholder = Lang["Password"];
        _Email.Placeholder = Lang["Email"];
        _Save.Text = Lang["Save"];
        _DeleteAccount.Text = Lang["DeleteAccount"];

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
            Text = Lang["Profile"],
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
        Content = _ContentLayout;
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
    }

    private async void Save(object sender, EventArgs e)
    {
        await _ProfileViewModel.Save(
            _Username.Text,
            _Password.Text,
            _Email.Text
        );

        await FetchUser();

        await DisplayAlert(
            Lang["Profile"],
            Lang["ChangesSaved"],
            Lang["Ok"]
        );
    }

    private async void DeleteAccount(object sender, EventArgs e)
    {
        bool shouldDelete = await DisplayAlert(
            Lang["AreYouSure"],
            Lang["ProfileDeletePrompt"],
            Lang["Yes"],
            Lang["No"]
        );

        if (shouldDelete)
        {
            await _ProfileViewModel.DeleteAccount();
            WeakReferenceMessenger.Default.Send<InternalMessage>(new InternalMessage("signed-out"));
        }
    }
    #endregion
}
