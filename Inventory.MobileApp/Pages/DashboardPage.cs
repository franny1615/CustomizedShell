using CommunityToolkit.Mvvm.Messaging;
using Inventory.MobileApp.Models;

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
				},
				new Button 
				{
					Text = "Log out",
					Command = new Command(() => { WeakReferenceMessenger.Default.Send(new InternalMsg(InternalMessage.LoggedOut)); })
				}
			}
		};
	}
}