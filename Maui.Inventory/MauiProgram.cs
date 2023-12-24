using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Inventory.Services;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using ZXing.Net.Maui.Controls;
using Maui.Inventory.Services.Interfaces;

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
		

		return builder;
	}

	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{

        return builder;
	}

	public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<ILanguageService, LanguageService>();
		builder.Services.AddTransient<IDAL<User>, UserDAL>();
		builder.Services.AddTransient<IDAL<Admin>, AdminDAL>();
		builder.Services.AddTransient<IDAL<ApiUrl>, ApiUrlDAL>();
		builder.Services.AddSingleton<IAPIService, APIService>();
		builder.Services.AddTransient<IAdminService, AdminService>();
		builder.Services.AddTransient<IUserService, UserService>();

		return builder;
	}
}
