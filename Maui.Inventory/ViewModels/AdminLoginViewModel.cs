using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory.ViewModels;

public partial class AdminLoginViewModel : ObservableObject
{
    private readonly ILanguageService _LangService;
    private readonly IAdminService _AdminService;
    public MaterialEntryModel Username = new();
    public MaterialEntryModel Password = new();

    public AdminLoginViewModel(
        ILanguageService languageService,
        IAdminService adminService)
    {
        _LangService = languageService;
        _AdminService = adminService;

        Username.Placeholder = _LangService.StringForKey("Username");
        Password.Placeholder = _LangService.StringForKey("Password");

        Username.PlaceholderIcon = MaterialIcon.Person;
        Password.PlaceholderIcon = MaterialIcon.Password;

        Username.Keyboard = Keyboard.Plain;
        Password.Keyboard = Keyboard.Plain;

        Username.IsSpellCheckEnabled = false;
        Password.IsSpellCheckEnabled = false;

        Password.IsPassword = true;
    }

    public void Clear()
    {
        Username.Text = "";
        Password.Text = "";
    }

    public async Task<bool> Login()
    {
        return await _AdminService.Login(Username.Text, Password.Text);
    }
}