using Maui.Inventory.Pages;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Inventory.ViewModels;
using Maui.Inventory.Services;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using ZXing.Net.Maui.Controls;

namespace Maui.Inventory;

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
			.UseBarcodeReader()
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
		builder.Services.AddTransient<BarcodesPage>();

		return builder;
	}

	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<DataViewModel>();
        builder.Services.AddTransient<CategoriesViewModel>();
		builder.Services.AddTransient<StatusesViewModel>();
		builder.Services.AddTransient<BarcodesViewModel>();

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
		builder.Services.AddTransient<IDAL<ApiUrl>, ApiUrlDAL>();
		builder.Services.AddSingleton<IAPIService, APIService>();

		return builder;
	}
}
