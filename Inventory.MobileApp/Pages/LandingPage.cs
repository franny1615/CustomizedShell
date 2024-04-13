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
	private readonly Image _LanguageSwitcher = new();
	private readonly Button _Login = new();
	private readonly Button _Register = new();
	private readonly TouchBehavior _SwitchLanguage = new() 
	{
        DefaultAnimationDuration = 250,
        DefaultAnimationEasing = Easing.CubicInOut,
        PressedOpacity = 0.75,
        PressedScale = 0.98
    };

	public LandingPage()
	{
		NavigationPage.SetHasNavigationBar(this, false);

		_SwitchLanguage.Command = new Command(() => this.DisplayLanguageSwitcher());
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
		_ContentLayout.Add(_LanguageSwitcher.Behaviors([_SwitchLanguage]).Top().End().ApplyMaterialIcon(MaterialIcon.Language, 32, Application.Current?.Resources["Primary"] as Color ?? Colors.White));
		_ContentLayout.Add(_Login.Text(LanguageService.Instance["Login"]).Row(1).Column(0));
		_ContentLayout.Add(_Register.Text(LanguageService.Instance["Register"]).Row(2).Column(0));

		Content = _ContentLayout;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LanguageChanged += UpdateLanguageStrings;
    }

	protected override void OnDisappearing()
	{
		LanguageChanged -= UpdateLanguageStrings;
		base.OnDisappearing();
	}

    private void UpdateLanguageStrings(object? sender, EventArgs e)
    {
		_LogoCaption.Text(LanguageService.Instance["Inventory Management"]);
		_Login.Text(LanguageService.Instance["Login"]);
		_Register.Text(LanguageService.Instance["Register"]);
    }
}