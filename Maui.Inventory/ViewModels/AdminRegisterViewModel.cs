using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory;

public partial class AdminRegisterViewModel : ObservableObject
{
    private readonly ILanguageService _LangService;
    private readonly IEmailService _EmailService;
    private readonly IAdminService _AdminService;

    public MaterialEntryModel Username = new();
    public MaterialEntryModel Password = new();
    public MaterialEntryModel Email = new();

    public MaterialEntryModel VerificationCode = new();

    public AdminRegisterViewModel(
        ILanguageService languageService,
        IEmailService emailService,
        IAdminService adminService)
    {
        _LangService = languageService;
        _EmailService = emailService;
        _AdminService = adminService;

        Username.Placeholder = _LangService.StringForKey("Username");
        Password.Placeholder = _LangService.StringForKey("Password");
        Email.Placeholder = _LangService.StringForKey("Email");
        VerificationCode.Placeholder = _LangService.StringForKey("VerificationCode");

        Username.PlaceholderIcon = MaterialIcon.Person;
        Password.PlaceholderIcon = MaterialIcon.Password;
        Email.PlaceholderIcon = MaterialIcon.Email;
        VerificationCode.PlaceholderIcon = MaterialIcon.Numbers;

        Username.Keyboard = Keyboard.Text;
        Password.Keyboard = Keyboard.Text;
        Email.Keyboard = Keyboard.Email;
        VerificationCode.Keyboard = Keyboard.Numeric;

        Username.IsSpellCheckEnabled = false;
        Password.IsSpellCheckEnabled = false;
        Email.IsSpellCheckEnabled = false;
        VerificationCode.IsSpellCheckEnabled = false;

        Password.IsPassword = true;
    }

    public void Clear()
    {
        Username.Text = string.Empty;
        Password.Text = string.Empty;
        Email.Text = string.Empty;
    }

    public async Task<bool> BeginEmailVerification()
    {
        return await _EmailService.BeginVerification(Email.Text);
    }

    public async Task<bool> VerifyCode()
    {
        if (int.TryParse(VerificationCode.Text, out int code))
        {
            return await _EmailService.Verify(Email.Text, code);
        }
        
        return false;
    }

    public async Task<RegistrationResponse> Register()
    {
        return await _AdminService.Register(
            username: Username.Text,
            password: Password.Text,
            email: Email.Text,
            true
        );
    }

    public async Task<bool> Login()
    {
        return await _AdminService.Login(
            username: Username.Text,
            password: Password.Text
        );
    }
}
