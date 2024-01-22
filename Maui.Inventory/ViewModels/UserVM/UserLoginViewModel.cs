using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Interfaces;
using Maui.Inventory.Models.UserModels;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory.ViewModels.UserVM;

public partial class UserLoginViewModel : ObservableObject
{
    private readonly IUserService _UserService;
    private readonly ILanguageService _LangService;
    private readonly IDAL<User> _UserDAL;

    public MaterialEntryModel AdminID = new();
    public MaterialEntryModel Username = new();
    public MaterialEntryModel Password = new();

    public UserLoginViewModel(
        ILanguageService languageService,
        IUserService userService,
        IDAL<User> userDAL)
    {
        _LangService = languageService;
        _UserService = userService;
        _UserDAL = userDAL;

        Username.Placeholder = _LangService.StringForKey("Username");
        Password.Placeholder = _LangService.StringForKey("Password");
        AdminID.Placeholder = _LangService.StringForKey("CompanyId");

        Username.PlaceholderIcon = MaterialIcon.Person;
        Password.PlaceholderIcon = MaterialIcon.Password;
        AdminID.PlaceholderIcon = MaterialIcon.Storefront;

        Username.Keyboard = Keyboard.Plain;
        Password.Keyboard = Keyboard.Plain;
        AdminID.Keyboard = Keyboard.Numeric;

        Username.IsSpellCheckEnabled = false;
        Password.IsSpellCheckEnabled = false;
        AdminID.IsSpellCheckEnabled = false;

        Password.IsPassword = true;
    }

    public void Clear()
    {
        AdminID.Text = "";
        Username.Text = "";
        Password.Text = "";
    }

    public async Task<bool> Login()
    {
        int.TryParse(AdminID.Text, out int adminId);

        bool signedIn = await _UserService.Login(
            adminId,
            Username.Text, 
            Password.Text);

        if (signedIn)
        {
            try
            {
                var user = (await _UserDAL.GetAll()).First();
                UIUtils.ToggleDarkMode(user.IsDarkModeOn);
            }
            catch { }
        }

        return signedIn;
    }
}
