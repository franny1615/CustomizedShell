using CustomizedShell.Pages;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;

namespace CustomizedShell;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitCore()
			.UseMauiCommunityToolkitMarkup()
			.RegisterPages()
			.ConfigureMauiHandlers(handlers =>
			{
				// leaving this here as an example
				// handlers.AddHandler<Shell, CShellRenderer>();
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSans");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		return builder.Build();
	}

	public static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<SearchPage>();
		builder.Services.AddTransient<SettingsPage>();

		return builder;
	}
}
