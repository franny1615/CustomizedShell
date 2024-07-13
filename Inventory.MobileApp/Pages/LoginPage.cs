using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Pages;

public class LoginPage : BasePage
{
	private readonly LoginViewModel _ViewModel;
	private readonly Grid _ContentContainer = new();
	private readonly ScrollView _Scroll = new() { Orientation = ScrollOrientation.Vertical };
	private readonly VerticalStackLayout _ContentLayout = new() { Spacing = 8, Padding = 8 };
	private readonly MaterialEntry _Username = new() { IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _Password = new() { IsSpellCheckEnabled = false, IsTextPredictionEnabled = false, IsPassword = true };
	private readonly Button _SubmitButton = new();

    private readonly VerticalStackLayout _LogoContainer = new();
    private readonly Image _Logo = new();
    private readonly Label _LogoCaption = new();

    private readonly HorizontalStackLayout _SwitcherContainer = new() { Spacing = 8 };
    private readonly Image _LanguageSwitcher = new();
    private readonly Image _ThemeSwitcher = new();
    private readonly TouchBehavior _SwitchLanguage = new()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.75,
        PressedScale = 0.98
    };
    private readonly TouchBehavior _SwitchTheme = new()
    {
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.75,
        PressedScale = 0.98
    };
	private readonly TouchBehavior _CreateAccountTap = new()
	{
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.75,
        PressedScale = 0.98
    };

	private readonly Label _CreateAccount = new();

    public LoginPage(LoginViewModel loginViewModel)
	{
		_ViewModel = loginViewModel;

		Title = LanguageService.Instance["Welcome"];

        _LanguageSwitcher
            .Behaviors([_SwitchLanguage])
            .ApplyMaterialIcon(MaterialIcon.Language, 32, Application.Current?.Resources["Primary"] as Color ?? Colors.White);
        _ThemeSwitcher
            .Behaviors([_SwitchTheme])
            .ApplyMaterialIcon(MaterialIcon.Dark_mode, 32, Application.Current?.Resources["Primary"] as Color ?? Colors.White);

        _SwitcherContainer.Add(_ThemeSwitcher);
        _SwitcherContainer.Add(_LanguageSwitcher);

        _SwitchLanguage.Command = new Command(() => this.DisplayLanguageSwitcher());
        _SwitchTheme.Command = new Command(() => this.DisplayThemeSwitcher());
		_CreateAccountTap.Command = new Command(async () =>
		{
			string company = LanguageService.Instance["Company Registration"];
			string user = LanguageService.Instance["User Registration"];
			string choice = await DisplayActionSheet(
				LanguageService.Instance["Options"],
				LanguageService.Instance["Cancel"],
				null,
				[company, user]);

			if (choice == company)
                await Navigation.PushAsync(PageService.Register(false));
			else if (choice == user)
                await Navigation.PushAsync(PageService.Register(true));
        });

        _LogoContainer.Spacing = 8;
        _LogoContainer.Add(_Logo
            .Source("app_ic.png")
            .Width(72)
            .Height(72)
            .Start());
        _LogoContainer.Add(_LogoCaption
            .Text(LanguageService.Instance["Inventory Management"])
            .Start()
            .FontSize(24)
            .Bold());

		_CreateAccount
			.Text(LanguageService.Instance["Create account"])
			.TextColor(UIService.Color("Primary"))
			.Padding(12)
			.FontSize(18)
			.Bold()
			.Behaviors([_CreateAccountTap]);

		_ContentLayout.Add(new Grid 
		{
			ColumnDefinitions = Columns.Define(Star, Auto),
			Children =
			{
				_LogoContainer.Column(0),
				_SwitcherContainer.Column(1).Top().End(),
			}
		});

        _ContentLayout.Add(_Username
			.Placeholder($"{LanguageService.Instance["Username"]}*")
			.PlaceholderIcon(MaterialIcon.Person));
		_ContentLayout.Add(_Password
			.Placeholder($"{LanguageService.Instance["Password"]}*")
			.PlaceholderIcon(MaterialIcon.Password));

		_ContentLayout.Add(_SubmitButton
			.Text(LanguageService.Instance["Login"]));
		_ContentLayout.Add(_CreateAccount.CenterHorizontal());

		_Scroll.Content = _ContentLayout;
		_ContentContainer.Add(_Scroll);

		Content = _ContentContainer;
	}

	protected override void OnAppearing()
    {
        base.OnAppearing();
		KeyboardService.Observe((overlap) => 
		{
			_ContentContainer.TranslationY = overlap == 0 ? 0 : -overlap;
		});
		_SubmitButton.Clicked += Submit;
        LanguageChanged += UpdateLanguageStrings;
    }

    protected override void OnDisappearing()
	{
		KeyboardService.EndObservation();
		_SubmitButton.Clicked -= Submit;
		LanguageChanged -= UpdateLanguageStrings;
		base.OnDisappearing();
	}

	private async void Submit(object? sender, EventArgs e)
	{
		if (_SubmitButton.Text == LanguageService.Instance["Logging In"] ||
			string.IsNullOrEmpty(_Username.Text) ||
			string.IsNullOrEmpty(_Password.Text))
		{
			return;
		}

		_SubmitButton.Text = LanguageService.Instance["Logging In"];
		bool loggedIn = await _ViewModel.Login(_Username.Text, _Password.Text);
		_SubmitButton.Text = LanguageService.Instance["Login"];

		if (loggedIn)
		{
			WeakReferenceMessenger.Default.Send(new InternalMsg(InternalMessage.LoggedIn));
		}
		else
		{
			await DisplayAlert(
				LanguageService.Instance["Incorrect username/password. Please try again."],
				"",
				LanguageService.Instance["OK"]
			);
			_Password.Text = "";
		}
	}

    private void UpdateLanguageStrings(object? sender, EventArgs e)
    {
		Title = LanguageService.Instance["Welcome"];
        _LogoCaption.Text(LanguageService.Instance["Inventory Management"]);
		_Username.Placeholder = $"{LanguageService.Instance["Username"]}*";
		_Password.Placeholder = $"{LanguageService.Instance["Password"]}*";
		_SubmitButton.Text = LanguageService.Instance["Login"];
		_CreateAccount.Text = LanguageService.Instance["Create account"];
    }
}