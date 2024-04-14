using System.Numerics;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Markup;
using Inventory.MobileApp.Services;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp.Pages;

public class LandingPage : BasePage
{
	private readonly Grid _ContentLayout = new()
	{
		RowDefinitions = Rows.Define(Star, Auto, Auto),
		RowSpacing = 24,
		Padding = new Thickness(12, 8, 12, 8)
	};
	private readonly VerticalStackLayout _LogoContainer = new();
	private readonly Image _Logo = new();
	private readonly Label _LogoCaption = new();
	private readonly HorizontalStackLayout _SwitcherContainer = new() { Spacing = 8 };
	private readonly Image _LanguageSwitcher = new();
	private readonly Image _ThemeSwitcher = new();
	private readonly Button _Login = new();
	private readonly Button _Register = new();
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

	public LandingPage()
	{
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
		_ContentLayout.Add(_LogoContainer.Top().Start().Row(0).Column(0));
		_ContentLayout.Add(_SwitcherContainer.Top().End());
		_ContentLayout.Add(_Login.Text(LanguageService.Instance["Login"]).Row(1).Column(0));
		_ContentLayout.Add(_Register.Text(LanguageService.Instance["Register"]).Row(2).Column(0));

		Content = _ContentLayout;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguageChanged += UpdateLanguageStrings;
		_Register.Clicked += GoRegister;
    }

	protected override void OnDisappearing()
	{
		LanguageChanged -= UpdateLanguageStrings;
		_Register.Clicked -= GoRegister;
		base.OnDisappearing();
	}

    private void UpdateLanguageStrings(object? sender, EventArgs e)
    {
		Title = LanguageService.Instance["Welcome"];
		_LogoCaption.Text(LanguageService.Instance["Inventory Management"]);
		_Login.Text(LanguageService.Instance["Login"]);
		_Register.Text(LanguageService.Instance["Register"]);
    }

	private void GoRegister(object? sender, EventArgs e)
	{
		Navigation.PushAsync(PageService.Register());
	}
}