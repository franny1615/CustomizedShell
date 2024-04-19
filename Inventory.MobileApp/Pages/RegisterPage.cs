using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Controls;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class RegisterPage : ContentPage
{
	private readonly RegisterViewModel _RegisterViewModel;
	private readonly Grid _ContentContainer = new();
	private readonly ScrollView _Scroll = new() { Orientation = ScrollOrientation.Vertical };
	private readonly VerticalStackLayout _ContentLayout = new() { Spacing = 8, Padding = 8 };
	private readonly Label _CompanyDetailsHeader = new();
	private readonly MaterialEntry _CompanyName = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _Address1 = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _Address2 = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _Address3 = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _Country = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _State = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _Zip = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Label _UserDetailsHeader = new();
	private readonly MaterialEntry _Username = new() {  Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _Password = new() {  Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false, IsPassword = true };
	private readonly MaterialEntry _Email = new() {  Keyboard = Keyboard.Email, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly MaterialEntry _PhoneNumber = new() {  Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Button _SubmitButton = new();

	public RegisterPage(RegisterViewModel registerViewModel)
	{
		_RegisterViewModel = registerViewModel;

		Title = LanguageService.Instance["Register"];

		_ContentLayout.Add(_CompanyDetailsHeader
			.Text(LanguageService.Instance["Company Details"])
			.FontSize(24)
			.Bold());
		_ContentLayout.Add(_CompanyName
			.Placeholder($"{LanguageService.Instance["Company Name"]}*")
			.PlaceholderIcon(MaterialIcon.Domain));
		_ContentLayout.Add(_Address1
			.Placeholder($"{LanguageService.Instance["Address 1"]}"));
		_ContentLayout.Add(_Address2
			.Placeholder($"{LanguageService.Instance["Address 2"]}"));
		_ContentLayout.Add(_Address3
			.Placeholder($"{LanguageService.Instance["Address 3"]}"));
		_ContentLayout.Add(_Country
			.Placeholder($"{LanguageService.Instance["Country"]}"));
		_ContentLayout.Add(_State
			.Placeholder($"{LanguageService.Instance["State"]}"));
		_ContentLayout.Add(_Zip
			.Placeholder($"{LanguageService.Instance["Zip"]}"));

		_ContentLayout.Add(_UserDetailsHeader
			.Text(LanguageService.Instance["Account Details"])
			.FontSize(24)
			.Bold());
		_ContentLayout.Add(_Username
			.Placeholder($"{LanguageService.Instance["Username"]}*")
			.PlaceholderIcon(MaterialIcon.Person));
		_ContentLayout.Add(_Password
			.Placeholder($"{LanguageService.Instance["Password"]}*")
			.PlaceholderIcon(MaterialIcon.Password));
		_ContentLayout.Add(_Email
			.Placeholder($"{LanguageService.Instance["Email"]}*")
			.PlaceholderIcon(MaterialIcon.Email));
		_ContentLayout.Add(_PhoneNumber
			.Placeholder($"{LanguageService.Instance["Phone Number"]}")
			.PlaceholderIcon(MaterialIcon.Phone));
		
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
		if (_SubmitButton.Text == LanguageService.Instance["Sending Code"] ||
			_SubmitButton.Text == LanguageService.Instance["Validating"])
		{
			return;
		}

		bool haveCompanyName = !string.IsNullOrEmpty(_CompanyName.Text);
		bool haveUsername = !string.IsNullOrEmpty(_Username.Text);
		bool havePassword = !string.IsNullOrEmpty(_Password.Text);
		bool haveEmail = !string.IsNullOrEmpty(_Email.Text);
		if (!haveCompanyName)		
			_CompanyName.ShowStatus(LanguageService.Instance["Required"], MaterialIcon.Info, Colors.Red);
		if (!haveUsername)
			_Username.ShowStatus(LanguageService.Instance["Required"], MaterialIcon.Info, Colors.Red);
		if (!havePassword)
			_Password.ShowStatus(LanguageService.Instance["Required"], MaterialIcon.Info, Colors.Red);
		if (!haveEmail)
			_Email.ShowStatus(LanguageService.Instance["Required"], MaterialIcon.Info, Colors.Red);
		if (!haveCompanyName || !haveUsername || !havePassword || !haveEmail)
			return;

		_SubmitButton.Text = LanguageService.Instance["Registering"];
		var response = await _RegisterViewModel.IsUsernameUnique(_Username.Text);
		_SubmitButton.Text = LanguageService.Instance["Submit"];

		if (!string.IsNullOrEmpty(response.ErrorMessage))
		{
			this.DisplayCommonError(response.ErrorMessage);
			return;
		}

		bool usernameUnique = response.Data;
		if (!usernameUnique)
		{
			await DisplayAlert(
				LanguageService.Instance["Username"],
				LanguageService.Instance["Please enter a unique username."],
				LanguageService.Instance["OK"]
			);
			_Username.Text = "";
			_Username.Focus();
			return;
		}

		bool sendEmailVerification = await DisplayAlert(
			LanguageService.Instance["Email Verification"],
			string.Format(LanguageService.Instance["EmailVerificationMessage"], _Email.Text),
			LanguageService.Instance["Continue"],
			LanguageService.Instance["Cancel"]);

		if (!sendEmailVerification)
		{
			return;
		}

		_SubmitButton.Text = LanguageService.Instance["Sending Code"];
		response = await _RegisterViewModel.BeginEmailValidation(_Email.Text);
		_SubmitButton.Text = LanguageService.Instance["Submit"];
		if (!string.IsNullOrEmpty(response.ErrorMessage))
		{
			this.DisplayCommonError(response.ErrorMessage);
			return;
		}

		string enteredCode = await DisplayPromptAsync(
			LanguageService.Instance["Email Verification"],
			string.Format(LanguageService.Instance["EnterVerifCode"], _Email.Text),
			LanguageService.Instance["Continue"],
			LanguageService.Instance["Cancel"]);

		_SubmitButton.Text = LanguageService.Instance["Validating"];
		response = await _RegisterViewModel.ValidateCode(_Email.Text, enteredCode);
		_SubmitButton.Text = LanguageService.Instance["Submit"];
        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            this.DisplayCommonError(response.ErrorMessage);
            return;
        }

		if (!response.Data)
		{
			await DisplayAlert(
				LanguageService.Instance["Email Verification"],
				LanguageService.Instance["Email verification failed."],
				LanguageService.Instance["OK"]);
			return;
		}

		await DisplayAlert(
			LanguageService.Instance["Email Verification"],
			LanguageService.Instance["Email has been verified."],
			LanguageService.Instance["OK"]);
    }
}