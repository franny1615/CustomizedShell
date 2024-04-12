using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Inventory.MobileApp;

public class LandingPage : ContentPage
{
	private readonly Grid _ContentLayout = new()
	{
		RowDefinitions = Rows.Define(Star, Auto, Auto),
		RowSpacing = 24,
		Padding = new Thickness(12, 8, 12, 8)
	};
	private readonly Image _Logo = new()
	{
		Source = "app_ic.png",
		WidthRequest = 72,
		HeightRequest = 72
	};
	private readonly Button _Login = new()
	{
		Text = "Login"
	};
	private readonly Button _Register = new()
	{
		Text = "Register"
	};

	public LandingPage()
	{
		NavigationPage.SetHasNavigationBar(this, false);

		_ContentLayout.Add(_Logo.Top().Start(), 0, 0);
		_ContentLayout.Add(_Login, 0, 1);
		_ContentLayout.Add(_Register, 0, 2);

		Content = _ContentLayout;
	}
}