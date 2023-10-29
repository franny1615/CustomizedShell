using CustomizedShell.Pages;
using Maui.Components.PlatformViews;

namespace CustomizedShell;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.RegisterPages()
			.ConfigureMauiHandlers(handlers =>
			{
				// handlers.AddHandler<Shell, CShellRenderer>();
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Montserrat-Regular.ttf", "MontserratRegular");
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
