using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Maui.Components;
using Maui.Inventory.Services;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using Maui.Inventory.ViewModels;
using Maui.Inventory.Pages.AdminPages;
using Maui.Inventory.Pages;
using Maui.Inventory.ViewModels.AdminVM;
using Maui.Inventory.Pages.UserPages;
using Maui.Inventory.ViewModels.UserVM;

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
			.RegisterPages()
			.RegisterViewModels()
			.RegisterServices()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", MaterialIcon.FontName);
			});

		return builder.Build();
	}

	public static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<SplashPage>();
		builder.Services.AddTransient<AdminDashboardPage>();
		builder.Services.AddTransient<AdminRegisterPage>();
		builder.Services.AddTransient<AdminEmailVerificationPage>();
		builder.Services.AddTransient<AdminLoginPage>();
		builder.Services.AddTransient<AdminUsersPage>();
		builder.Services.AddTransient<AdminProfilePage>();

		builder.Services.AddTransient<UserLoginPage>();
		builder.Services.AddTransient<UserProfilePage>();

		builder.Services.AddTransient<InventoryPage>();
		builder.Services.AddTransient<AdminStatusesPage>();
		builder.Services.AddTransient<AdminLocationsPage>();
		builder.Services.AddTransient<AdminEditLocationPage>();
		builder.Services.AddTransient<AdminQuantityTypesPage>();
		builder.Services.AddTransient<SendFeedbackPage>();

		return builder;
	}

	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<SplashViewModel>();
		builder.Services.AddTransient<AdminRegisterViewModel>();
		builder.Services.AddTransient<AppViewModel>();
		builder.Services.AddTransient<AdminLoginViewModel>();
		builder.Services.AddTransient<AdminProfileViewModel>();
		builder.Services.AddTransient<AdminUpdateEmailViewModel>();
		builder.Services.AddTransient<AdminResetPasswordViewModel>();
		builder.Services.AddTransient<AdminUsersViewModel>();

		builder.Services.AddTransient<UserLoginViewModel>();
		builder.Services.AddTransient<UserProfileViewModel>();

		builder.Services.AddTransient<InventoryViewModel>();
		builder.Services.AddTransient<AdminStatusesViewModel>();
		builder.Services.AddTransient<AdminLocationsViewModel>();
		builder.Services.AddTransient<AdminQuantityTypesViewModel>();
		builder.Services.AddTransient<SendFeedbackViewModel>();

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
		builder.Services.AddTransient<IEmailService, EmailService>();
		builder.Services.AddTransient<IInventoryService, InventoryService>();
        builder.Services.AddTransient<ILocationsService, LocationsService>();
        builder.Services.AddTransient<ICRUDService<Status>, StatusService>();
		builder.Services.AddTransient<ICRUDService<QuantityType>, QuantityTypesService>();

        return builder;
	}
}
