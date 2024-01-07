using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory;

public partial class AdminUpdateEmailViewModel : ObservableObject
{
    private readonly ILanguageService _LangService;
    private readonly IDAL<Admin> _AdminDAL;
    private readonly IEmailService _EmailService;
    private readonly IAdminService _AdminService;

    public MaterialEntryModel Email = new();
    public MaterialEntryModel VerificationCode = new();

    public AdminUpdateEmailViewModel(
        ILanguageService languageService,
        IDAL<Admin> adminDAL,
        IEmailService emailService,
        IAdminService adminService)
    {
        _LangService = languageService;
        _AdminDAL = adminDAL;
        _EmailService = emailService;
        _AdminService = adminService;   

        Email.Placeholder = _LangService.StringForKey("Email");
        Email.PlaceholderIcon = MaterialIcon.Email;
        Email.Keyboard = Keyboard.Email;
        Email.IsSpellCheckEnabled = false;

        VerificationCode.Placeholder = _LangService.StringForKey("VerificationCode");
        VerificationCode.PlaceholderIcon = MaterialIcon.Numbers;
        VerificationCode.Keyboard = Keyboard.Numeric;
        VerificationCode.IsSpellCheckEnabled = false;
    }

    public async Task<bool> SendCode()
    {
        return await _EmailService.BeginVerification(Email.Text);
    }

    public async Task<bool> VerifyCode()
    {
        bool parsedInt = int.TryParse(VerificationCode.Text, out int result);
        return parsedInt ? await _EmailService.Verify(Email.Text, result) : false;
    }

    public async Task<bool> SaveEmail()
    {
        try
        {
            var admin = (await _AdminDAL.GetAll()).First();
            admin.Email = Email.Text;

            await _AdminDAL.Update(admin);

            return await _AdminService.UpdateAdmin(admin);
        }
        catch (Exception ex)
        {
            // TODO: add logging
#if DEBUG
            System.Diagnostics.Debug.WriteLine(ex);
#endif
            return false;
        }
    }
}
