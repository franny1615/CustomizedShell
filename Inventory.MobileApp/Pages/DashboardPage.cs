namespace Inventory.MobileApp.Pages;

public class DashboardPage : BasePage
{
	public DashboardPage()
	{
		Content = new VerticalStackLayout
		{
			Children = 
			{
				new Label 
				{ 
					HorizontalOptions = LayoutOptions.Center, 
					VerticalOptions = LayoutOptions.Center, 
					Text = "Welcome to .NET MAUI!"
				}
			}
		};
	}
}