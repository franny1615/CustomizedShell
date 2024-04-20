using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class LoginPage : BasePage
{
	private readonly LoginViewModel _ViewModel;
	private readonly Grid _ContentContainer = new();
	private readonly ScrollView _Scroll = new() { Orientation = ScrollOrientation.Vertical };
	private readonly VerticalStackLayout _ContentLayout = new() { Spacing = 8, Padding = 8 };
	private readonly Label _UserDetailsHeader = new();
	private readonly MaterialEntry _Username = new() {  Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _Password = new() {  Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false, IsPassword = true };
	private readonly Button _SubmitButton = new();

	public LoginPage(LoginViewModel loginViewModel)
	{
		_ViewModel = loginViewModel;

		Title = LanguageService.Instance["Login"];

		_ContentLayout.Add(_UserDetailsHeader
			.Text(LanguageService.Instance["Enter Credentials"])
			.FontSize(24)
			.Bold());
		_ContentLayout.Add(_Username
			.Placeholder($"{LanguageService.Instance["Username"]}*")
			.PlaceholderIcon(MaterialIcon.Person));
		_ContentLayout.Add(_Password
			.Placeholder($"{LanguageService.Instance["Password"]}*")
			.PlaceholderIcon(MaterialIcon.Password));

		_ContentLayout.Add(_SubmitButton
			.Text(LanguageService.Instance["Submit"]));

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
    }

	protected override void OnDisappearing()
	{
		KeyboardService.EndObservation();
		_SubmitButton.Clicked -= Submit;
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
		_SubmitButton.Text = LanguageService.Instance["Submit"];

		if (loggedIn)
		{
			await Navigation.PushAsync(PageService.Dashboard());
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
}