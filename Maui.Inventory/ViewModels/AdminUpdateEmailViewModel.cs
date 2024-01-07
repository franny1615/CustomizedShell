using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;
using Maui.Inventory.Services;

namespace Maui.Inventory;

public partial class AdminUpdateEmailViewModel : ObservableObject
{
    private readonly ILanguageService _LangService;
    private readonly IDAL<Admin> _AdminDAL;
    private readonly AdminService _AdminService;

    public MaterialEntryModel Email = new();
    public MaterialEntryModel VerificationCode = new();
    public MaterialEntryModel Password = new();

    public AdminUpdateEmailViewModel(
        ILanguageService languageService,
        IDAL<Admin> adminDAL,
        AdminService adminService)
    {
        _LangService = languageService;
        _AdminDAL = adminDAL;
        _AdminService = adminService;   

        Email.Placeholder = _LangService.StringForKey("Email");
        Email.PlaceholderIcon = MaterialIcon.Email;
        Email.Keyboard = Keyboard.Email;
        Email.IsSpellCheckEnabled = false;

        VerificationCode.Placeholder = _LangService.StringForKey("VerificationCode");
        VerificationCode.PlaceholderIcon = MaterialIcon.Numbers;
        VerificationCode.Keyboard = Keyboard.Numeric;
        VerificationCode.IsSpellCheckEnabled = false;

        Password.Placeholder = _LangService.StringForKey("Password");
        Password.PlaceholderIcon = MaterialIcon.Password;
        Password.Keyboard = Keyboard.Plain;
        Password.IsSpellCheckEnabled = false;
        Password.IsPassword = true;
    }
}
