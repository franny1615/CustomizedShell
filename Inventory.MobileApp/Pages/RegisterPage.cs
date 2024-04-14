using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.Pages;

public class RegisterPage : ContentPage
{
	private readonly VerticalStackLayout _ContentLayout = new() { Spacing = 8, Padding = 8 };
	private readonly Label _CompanyDetailsHeader = new();
	private readonly Entry _CompanyName = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Address1 = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Address2 = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Address3 = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Country = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _State = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Zip = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Label _UserDetailsHeader = new();
	private readonly Entry _Username = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _Password = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false, IsPassword = true };
	private readonly Entry _Email = new() { Keyboard = Keyboard.Email, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Entry _PhoneNumber = new() { Keyboard = Keyboard.Plain, IsSpellCheckEnabled = false, IsTextPredictionEnabled = false };
	private readonly Button _SubmitButton = new();

	public RegisterPage()
	{
		Title = LanguageService.Instance["Register"];

		_ContentLayout.Add(_CompanyDetailsHeader
			.Text(LanguageService.Instance["Company Details"])
			.FontSize(24)
			.Bold());
		_ContentLayout.Add(_CompanyName
			.Placeholder(LanguageService.Instance["Company Name"]));
		_ContentLayout.Add(_Address1
			.Placeholder(LanguageService.Instance["Address 1"]));
		_ContentLayout.Add(_Address2
			.Placeholder(LanguageService.Instance["Address 2"]));
		_ContentLayout.Add(_Address3
			.Placeholder(LanguageService.Instance["Address 3"]));
		_ContentLayout.Add(_Country
			.Placeholder(LanguageService.Instance["Country"]));
		_ContentLayout.Add(_State
			.Placeholder(LanguageService.Instance["State"]));
		_ContentLayout.Add(_Zip
			.Placeholder(LanguageService.Instance["Zip"]));

		_ContentLayout.Add(_UserDetailsHeader
			.Text(LanguageService.Instance["Account Details"])
			.FontSize(24)
			.Bold());
		_ContentLayout.Add(_Username
			.Placeholder(LanguageService.Instance["Username"]));
		_ContentLayout.Add(_Password
			.Placeholder(LanguageService.Instance["Password"]));
		_ContentLayout.Add(_Email
			.Placeholder(LanguageService.Instance["Email"]));
		_ContentLayout.Add(_PhoneNumber
			.Placeholder(LanguageService.Instance["Phone Number"]));
		
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

	private void Submit(object? sender, EventArgs e)
	{
		Toast.Make("test").Show();
	}
}