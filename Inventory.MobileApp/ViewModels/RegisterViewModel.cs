using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.ViewModels;

public class RegisterViewModel
{
    private int _UserID = 0;
    private int _CompanyID = 0;

    public async Task<NetworkResponse<bool>> BeginEmailValidation(string email)
    {
        return await NetworkService.Post<bool>(Endpoints.beginEmailValidation, new { email });
    }

    public async Task<NetworkResponse<bool>> ValidateCode(string email, string codeStr)
    {
        int.TryParse(codeStr, out int codeInt);
        return await NetworkService.Post<bool>(Endpoints.validateEmail, new { email, code = codeInt });
    }

    public async Task<NetworkResponse<bool>> DoesUsernameExist(string username)
    {
        return await NetworkService.Get<bool>(Endpoints.checkUsername, new() 
        { 
            { "userName", username }
        });
    }

    public async Task<NetworkResponse<int>> RegisterUser(
        string userName,
        string password,
        string email,
        string phoneNumber)
    {
        var response = await NetworkService.Post<int>(Endpoints.registerUser, new 
        {
            companyID = _CompanyID,
            userName,
            password,
            isDarkModeOn = SessionService.CurrentTheme == "dark",
            localization = SessionService.CurrentLanguageCulture,
            email,
            phoneNumber,
            isCompanyOwner = true
        });
        _UserID = response.Data;
        return response;
    }

    public async Task<NetworkResponse<int>> RegisterCompany(
        string name,
        string address1,
        string address2,
        string address3,
        string country,
        string city,
        string state,
        string zip)
    {
        var response = await NetworkService.Post<int>(Endpoints.registerCompany, new 
        {
            name,
            address1,
            address2,
            address3,
            country,
            city,
            state,
            zip
        });

        if (string.IsNullOrEmpty(response.ErrorMessage))
        {
            _CompanyID = response.Data;
        }

        return response;
    }

    public async Task<bool> Login(string userName, string password)
    {
        var response = await NetworkService.Post<string>(Endpoints.login, new 
        {
            userName,
            password
        });
        SessionService.AuthToken = response.Data ?? "";

        // owner permissions
        var permsresp = await NetworkService.Post<int>(Endpoints.insertPermission, new
        {
            UserId = _UserID,
            CompanId = _CompanyID,
            InventoryPermissions = 1023 // all perms...
        });
        response.ErrorMessage = $"{response.ErrorMessage ?? ""}{permsresp.ErrorMessage ?? ""}";

        return !string.IsNullOrEmpty(response.Data);
    }
}
