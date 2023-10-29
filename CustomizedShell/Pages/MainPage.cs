using Maui.Components.Controls;

namespace CustomizedShell.Pages;

public class MainPage : ContentPage
{
	public MainPage()
	{
		Content = new Grid
		{
			Children = 
			{
				new Label 
				{ 
					HorizontalOptions = LayoutOptions.Center, 
					VerticalOptions = LayoutOptions.Center, 
					Text = "Home Page"
				},
				new VerticalStackLayout
				{
					VerticalOptions = LayoutOptions.End,
					HorizontalOptions = LayoutOptions.End,
					Margin = 16,
					Padding = 0,
					Spacing = 16,
					Children =
					{ 						
						new FloatingActionButton
						{
							FABBackgroundColor = Color.FromArgb("#3EB489"),
							ImageSource = "home.png",
							FABStyle = FloatingActionButtonStyle.Small,
						},
						new FloatingActionButton
						{
							FABBackgroundColor = Color.FromArgb("#3EB489"),
							ImageSource = "home.png",
							FABStyle = FloatingActionButtonStyle.Regular,
						},
						new FloatingActionButton
						{
							FABBackgroundColor = Color.FromArgb("#3EB489"),
							ImageSource = "home.png",
							FABStyle = FloatingActionButtonStyle.Large,
						}
					}
				}
			}
		};
	}
}