using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Inventory.Services;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using ZXing.Net.Maui.Controls;
using Maui.Inventory.Services.Interfaces;
using Maui.Inventory.Models.AdminModels;
using Maui.Inventory.Models.UserModels;

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
			.RegisterServices()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", MaterialIcon.FontName);
			});
		
		builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		return builder.Build();
	}

	public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<IDAL<User>, UserDAL>();
		builder.Services.AddTransient<IDAL<Admin>, AdminDAL>();
		builder.Services.AddTransient<IDAL<ApiUrl>, ApiUrlDAL>();
		builder.Services.AddSingleton<IAPIService, APIService>();
		builder.Services.AddTransient<IAdminService, AdminService>();
		builder.Services.AddTransient<IUserService, UserService>();
		builder.Services.AddTransient<IEmailService, EmailService>();

		return builder;
	}
}
