using CustomizedShell.Pages;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Maui.Components;
using CustomizedShell.ViewModels;
using CustomizedShell.Services;
using Maui.Components.Interfaces;
using CustomizedShell.Models;

namespace CustomizedShell;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiComponents()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitCore()
			.UseMauiCommunityToolkitMarkup()
			.RegisterPages()
			.RegisterViewModels()
			.RegisterServices()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		return builder.Build();
	}

	public static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<ProfilePage>();
		builder.Services.AddTransient<InventoryPage>();
		builder.Services.AddTransient<DataPage>();
		builder.Services.AddTransient<StatusesPage>();
		builder.Services.AddTransient<CategoriesPage>();

		return builder;
	}

	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<DataViewModel>();
        builder.Services.AddTransient<CategoriesViewModel>();
		builder.Services.AddTransient<StatusesViewModel>();

        return builder;
	}

	public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<ILanguageService, LanguageService>();
		builder.Services.AddTransient<IDAL<Category>, CategoryDAL>();
		builder.Services.AddTransient<IDAL<InventoryItem>, InventoryItemDAL>();
		builder.Services.AddTransient<IDAL<Status>, StatusDAL>();
		builder.Services.AddTransient<IDAL<Barcode>, BarcodeDAL>();
		builder.Services.AddTransient<IDAL<User>, UserDAL>();

		return builder;
	}
}
