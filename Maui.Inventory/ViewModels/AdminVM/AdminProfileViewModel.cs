using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Interfaces;
using Maui.Inventory.Models.AdminModels;
using Maui.Inventory.Services.Interfaces;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory.ViewModels;

public partial class AdminProfileViewModel : ObservableObject
{
    private readonly IAdminService _AdminService;
    private readonly ILanguageService _LangService;
    private readonly IEmailService _EmailService;
    private readonly IDAL<Admin> _AdminDAL;
    public MaterialEntryModel Username = new();
    public MaterialEntryModel Email = new();
    public MaterialEntryModel CompanyId = new();
    public MaterialEntryModel LicenseId = new();
    
    [ObservableProperty]
    public bool isDarkModeOn = false;

    public AdminProfileViewModel(
        IAdminService adminService,
        ILanguageService langService,
        IEmailService emailService,
        IDAL<Admin> adminDAL)
    {
        _AdminService = adminService;
        _LangService = langService;
        _AdminDAL = adminDAL;
        _EmailService = emailService;

        Username.Placeholder = _LangService.StringForKey("Username");
        Email.Placeholder = _LangService.StringForKey("Email");
        CompanyId.Placeholder = _LangService.StringForKey("CompanyId");
        LicenseId.Placeholder = _LangService.StringForKey("LicenseId");

        Username.PlaceholderIcon = MaterialIcon.Person;
        Email.PlaceholderIcon = MaterialIcon.Email;
        CompanyId.PlaceholderIcon = MaterialIcon.Storefront;
        LicenseId.PlaceholderIcon = MaterialIcon.Folder;
    }

    public async Task GetProfile()
    {
        try
        {
            var admins = await _AdminDAL.GetAll();
            var admin = admins.First();
            Username.Text = admin.UserName;
            Email.Text = admin.Email;
            CompanyId.Text = admin.Id.ToString();
            LicenseId.Text = admin.LicenseID.ToString();

            IsDarkModeOn = admin.IsDarkModeOn;
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }
    }

    public async Task SaveDarkModeSettings()
    {
        var admins = await _AdminDAL.GetAll();
        var admin = admins.First();

        admin.IsDarkModeOn = IsDarkModeOn;

        await _AdminDAL.Update(admin);
        await _AdminService.UpdateAdmin(admin);
    }

    public async Task Logout()
    {
        try
        {
            await _AdminDAL.DeleteAll();
        } catch { }
    }

    public async Task<bool> SendCode()
    {
        try
        {
            var admins = await _AdminDAL.GetAll();
            var admin = admins.First();

            return await _EmailService.BeginVerification(admin.Email);
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }

        return false;
    }
}