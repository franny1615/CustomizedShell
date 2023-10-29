namespace CustomizedShell.Pages;

public class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		Content = new VerticalStackLayout
		{
			Children = 
			{
				new Label 
				{
					HorizontalOptions = LayoutOptions.Center, 
					VerticalOptions = LayoutOptions.Center, 
					Text = "Settings Page"
				}
			}
		};
	}
}