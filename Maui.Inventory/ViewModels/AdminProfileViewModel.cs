using CommunityToolkit.Mvvm.ComponentModel;
using Maui.Components;
using Maui.Components.Interfaces;
using Maui.Inventory.Models;

namespace Maui.Inventory.ViewModels;

public partial class AdminProfileViewModel : ObservableObject
{
    private readonly ILanguageService _LangService;
    private readonly IDAL<Admin> _AdminDAL;
    public MaterialEntryModel Username = new();
    public MaterialEntryModel Email = new();
    public MaterialEntryModel CompanyId = new();
    public MaterialEntryModel LicenseId = new();

    public AdminProfileViewModel(
        ILanguageService langService,
        IDAL<Admin> adminDAL)
    {
        _LangService = langService;
        _AdminDAL = adminDAL;

        Username.Placeholder = _LangService.StringForKey("Username");
        Email.Placeholder = _LangService.StringForKey("Email");
        CompanyId.Placeholder = _LangService.StringForKey("CompanyId");
        LicenseId.Placeholder = _LangService.StringForKey("LicenseId");

        Username.PlaceholderIcon = MaterialIcon.Person;
        Email.PlaceholderIcon = MaterialIcon.Email;
        CompanyId.PlaceholderIcon = MaterialIcon.Numbers;
        LicenseId.PlaceholderIcon = MaterialIcon.Numbers;
    }

    public async Task GetProfile()
    {
        try
        {
            var admin = (await _AdminDAL.GetAll()).First();
            Username.Text = admin.UserName;
            Email.Text = admin.Email;
            CompanyId.Text = admin.Id.ToString();
            LicenseId.Text = admin.LicenseID.ToString();
        }
        catch (Exception ex)
        {
            // TODO: add logging
        }
    }

    public async Task Logout()
    {
        try
        {
            await _AdminDAL.DeleteAll();
        } catch { }
    }
}