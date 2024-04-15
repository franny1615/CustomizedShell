using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
using Inventory.MobileApp.ViewModels;

namespace Inventory.MobileApp.Pages;

public class RegisterPage : ContentPage
{
	private readonly RegisterViewModel _RegisterViewModel;
	private readonly VerticalStackLayout _ContentLayout = new() { Spacing = 8, Padding = 8 };
	private readonly Label _CompanyDetailsHeader = new();
	private readonly Entry _CompanyName = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Address1 = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Address2 = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Address3 = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Country = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _State = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Zip = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Label _UserDetailsHeader = new();
	private readonly Entry _Username = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Password = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false, IsPassword = true };
	private readonly Entry _Email = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Email, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _PhoneNumber = new() { Margin = new Thickness(8, 0, 0, 0), Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
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
			.Placeholder($"{LanguageService.Instance["Company Name"]} ({LanguageService.Instance["Required"]})"));
		_ContentLayout.Add(_Address1
			.Placeholder($"{LanguageService.Instance["Address 1"]} ({LanguageService.Instance["Optional"]})"));
		_ContentLayout.Add(_Address2
			.Placeholder($"{LanguageService.Instance["Address 2"]} ({LanguageService.Instance["Optional"]})"));
		_ContentLayout.Add(_Address3
			.Placeholder($"{LanguageService.Instance["Address 3"]} ({LanguageService.Instance["Optional"]})"));
		_ContentLayout.Add(_Country
			.Placeholder($"{LanguageService.Instance["Country"]} ({LanguageService.Instance["Optional"]})"));
		_ContentLayout.Add(_State
			.Placeholder($"{LanguageService.Instance["State"]} ({LanguageService.Instance["Optional"]})"));
		_ContentLayout.Add(_Zip
			.Placeholder($"{LanguageService.Instance["Zip"]} ({LanguageService.Instance["Optional"]})"));

		_ContentLayout.Add(_UserDetailsHeader
			.Text(LanguageService.Instance["Account Details"])
			.FontSize(24)
			.Bold());
		_ContentLayout.Add(_Username
			.Placeholder($"{LanguageService.Instance["Username"]} ({LanguageService.Instance["Required"]})"));
		_ContentLayout.Add(_Password
			.Placeholder($"{LanguageService.Instance["Password"]} ({LanguageService.Instance["Required"]})"));
		_ContentLayout.Add(_Email
			.Placeholder($"{LanguageService.Instance["Email"]} ({LanguageService.Instance["Required"]})"));
		_ContentLayout.Add(_PhoneNumber
			.Placeholder($"{LanguageService.Instance["Phone Number"]} ({LanguageService.Instance["Optional"]})"));
		
		_ContentLayout.Add(_SubmitButton
			.Text(LanguageService.Instance["Submit"]));

		Content = new ScrollView 
		{
			Content = _ContentLayout, 
			Orientation = ScrollOrientation.Vertical 
		};
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_SubmitButton.Clicked += Submit;
    }

	protected override void OnDisappearing()
	{
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

		if (string.IsNullOrEmpty(_CompanyName.Text))
		{
			await DisplayAlert(
				LanguageService.Instance["Company Name"], 
				LanguageService.Instance["Required"], 
				LanguageService.Instance["OK"]);
			return;
		}

		if (string.IsNullOrEmpty(_Username.Text))
		{
			await DisplayAlert(
				LanguageService.Instance["Username"],
				LanguageService.Instance["Required"],
				LanguageService.Instance["OK"]);
			return;
		}

		if (string.IsNullOrEmpty(_Password.Text))
		{
			await DisplayAlert(
				LanguageService.Instance["Password"],
				LanguageService.Instance["Required"],
				LanguageService.Instance["OK"]);
			return;
		}

		if (string.IsNullOrEmpty(_Email.Text))
		{
			await DisplayAlert(
				LanguageService.Instance["Email"],
				LanguageService.Instance["Required"],
				LanguageService.Instance["OK"]);
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
		var response = await _RegisterViewModel.BeginEmailValidation(_Email.Text);
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