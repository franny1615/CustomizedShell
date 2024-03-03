using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Controls;
using Maui.Components.Pages;
using Maui.Inventory.Services.Interfaces;
using Microsoft.AppCenter.Crashes;
using System.Globalization;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;

namespace Maui.Inventory.Pages.AdminPages;

public class AdminManageSubscriptionPage : BasePage
{
    #region Private Properties
    private readonly ILanguageService _langService;
    private AdminManageSubscriptionViewModel _ViewModel => (AdminManageSubscriptionViewModel)BindingContext;
    private readonly MaterialEntry _licenseExpirationDate;
    private readonly HorizontalRule _purchase = new();
    private readonly FloatingActionButton _oneMonthLicense = new()
    {
        FABBackgroundColor = Application.Current.Resources["Primary"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly FloatingActionButton _sixMonthLicense = new()
    {
        FABBackgroundColor = Application.Current.Resources["PrimaryShade"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended
    };
    private readonly FloatingActionButton _twelveMonthLicense = new()
    {
        FABBackgroundColor = Application.Current.Resources["Secondary"] as Color,
        TextColor = Colors.White,
        FABStyle = FloatingActionButtonStyle.Extended
    };
    #endregion

    #region Constructor
    public AdminManageSubscriptionPage(
        ILanguageService languageService,
        AdminManageSubscriptionViewModel viewModel) : base(languageService)
    {
        BindingContext = viewModel;
        _langService = languageService;
        _licenseExpirationDate = new(viewModel.LicenseExpirationDate);
        _licenseExpirationDate.IsDisabled = true;

        Title = languageService.StringForKey("Subscription");
        _purchase.Text = languageService.StringForKey("Purchase Extension");
        _oneMonthLicense.Text = languageService.StringForKey("One Month");
        _sixMonthLicense.Text = languageService.StringForKey("Six Month");
        _twelveMonthLicense.Text = languageService.StringForKey("Twelve Month");

        Content = new Grid
        {
            RowDefinitions = Rows.Define(Star, Auto, Auto, Auto, Auto),
            RowSpacing = 16,
            Padding = 16,
            Children =
            {
                _licenseExpirationDate.Row(0).Top(),
                _purchase.Row(1),
                _oneMonthLicense.Row(2),
                _sixMonthLicense.Row(3),
                _twelveMonthLicense.Row(4)
            }
        };

        _oneMonthLicense.Clicked += OneMonth;
        _sixMonthLicense.Clicked += SixMonth;
        _twelveMonthLicense.Clicked += TwelveMonth;
    }
    ~AdminManageSubscriptionPage()
    {
        _oneMonthLicense.Clicked -= OneMonth;
        _sixMonthLicense.Clicked -= SixMonth;
        _twelveMonthLicense.Clicked -= TwelveMonth;
    }
    #endregion

    #region Overrides
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _ViewModel.GetProfile();
    }
    #endregion

    #region Helpers
    private void TwelveMonth(object sender, ClickedEventArgs e)
    {
        Purchase(_oneMonthLicense.Text, 12);
    }

    private void SixMonth(object sender, ClickedEventArgs e)
    {
        Purchase(_sixMonthLicense.Text, 6);
    }

    private void OneMonth(object sender, ClickedEventArgs e)
    {
        Purchase(_twelveMonthLicense.Text, 1);
    }

    private async void Purchase(string currentButtonText, int amount)
    {
        if (currentButtonText == _langService.StringForKey("Purchasing"))
        {
            return;
        }

        bool success = await _ViewModel.UpdateLicense(amount);
        if (success)
        {
            await DisplayAlert(
                _langService.StringForKey("Success"), 
                string.Format(_langService.StringForKey("LicenseExtension"), amount),
                _langService.StringForKey("OK"));
        }
        else
        {
            await DisplayAlert(
                _langService.StringForKey("Error"),
                _langService.StringForKey("LicenseExtensionFailure"),
                _langService.StringForKey("OK"));
        }

        await Shell.Current.GoToAsync("..");
    }
    #endregion
}

public class AdminManageSubscriptionViewModel : ObservableObject
{
    private readonly IAdminService _AdminService;
    private readonly ILanguageService _LangService;
    private readonly IDAL<Admin> _AdminDAL;

    public MaterialEntryModel LicenseExpirationDate = new();

    public AdminManageSubscriptionViewModel(
        IAdminService adminService, 
        ILanguageService langService,
        IDAL<Admin> adminDAL)
    {
        _AdminService = adminService;
        _LangService = langService;
        _AdminDAL = adminDAL;

        LicenseExpirationDate.Placeholder = _LangService.StringForKey("License Expiration Date");
        LicenseExpirationDate.PlaceholderIcon = MaterialIcon.Calendar_today;
    }

    public async Task GetProfile()
    {
        try
        {
            var admins = await _AdminDAL.GetAll();
            var admin = admins.First();
            LicenseExpirationDate.Text = admin.LicenseExpirationDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }
    }

    public async Task<bool> UpdateLicense(int byMonths)
    {
        bool apiResult = await _AdminService.UpdateLicense(byMonths);

        if (apiResult)
        {
            var admins = await _AdminDAL.GetAll();
            var admin = admins.First();
            
            var updatedLicense = admin.LicenseExpirationDate.AddMonths(byMonths);
            admin.LicenseExpirationDate = updatedLicense;

            await _AdminDAL.Save(admin);
        }

        return apiResult;
    }
}

