using Inventory.MobileApp.Models;
using Inventory.MobileApp.Services;

namespace Inventory.MobileApp.ViewModels;

public class RegisterViewModel
{
    public async Task<NetworkResponse<bool>> BeginEmailValidation(string email)
    {
        return await NetworkService.Post<bool>(Endpoints.beginEmailValidation, new { email });
    }

    public async Task<NetworkResponse<bool>> ValidateCode(string email, string codeStr)
    {
        int.TryParse(codeStr, out int codeInt);
        return await NetworkService.Post<bool>(Endpoints.validateEmail, new { email, code = codeInt });
    }
}
