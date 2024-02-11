using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;
using Microsoft.AppCenter.Crashes;

namespace Maui.Inventory.ViewModels.AdminVM;

public partial class AdminResetPasswordViewModel : ObservableObject
{
    private readonly ILanguageService _LangService;
    private readonly IDAL<Admin> _AdminDAL;
    private readonly IEmailService _EmailService;
    private readonly IAdminService _AdminService;

    public MaterialEntryModel VerificationCode = new();
    public MaterialEntryModel NewPassword = new();

    public Admin CurrentAdmin = null;

    public AdminResetPasswordViewModel(
        ILanguageService languageService,
        IDAL<Admin> adminDAL,
        IEmailService emailService,
        IAdminService adminService)
    {
        _LangService = languageService;
        _AdminDAL = adminDAL;
        _EmailService = emailService;
        _AdminService = adminService;

        VerificationCode.Placeholder = _LangService.StringForKey("VerificationCode");
        VerificationCode.PlaceholderIcon = MaterialIcon.Numbers;
        VerificationCode.Keyboard = Keyboard.Numeric;
        VerificationCode.IsSpellCheckEnabled = false;

        NewPassword.Placeholder = _LangService.StringForKey("NewPassword");
        NewPassword.PlaceholderIcon = MaterialIcon.Password;
        NewPassword.Keyboard = Keyboard.Plain;
        NewPassword.IsSpellCheckEnabled = false;
        NewPassword.IsPassword = true;
    }

    public async Task GetDetails()
    {
        try
        {
            CurrentAdmin = (await _AdminDAL.GetAll()).First();
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }
    }

    public async Task<bool> SendCode()
    {
        return await _EmailService.BeginVerification(CurrentAdmin.Email);
    }

    public async Task<bool> VerifyCode()
    {
        bool parsedInt = int.TryParse(VerificationCode.Text, out int result);
        return parsedInt ? await _EmailService.Verify(CurrentAdmin.Email, result) : false;
    }

    public async Task<bool> SavePassword()
    {
        CurrentAdmin.Password = NewPassword.Text;
        return await _AdminService.UpdateAdmin(CurrentAdmin);
    }
}
