
using Maui.Inventory.Models;
using Maui.Inventory.Services.Interfaces;

namespace Maui.Inventory;

public class EmailService : IEmailService
{
    private readonly IAPIService _APIService;

    public EmailService(IAPIService apiService)
    {
        _APIService = apiService;
    }

    public async Task<bool> BeginVerification(string eml)
    {
        return await _APIService.Post<bool>(Endpoint.EmailStartVerifying, new
        {
            email = eml
        });
    }

    public async Task<bool> Verify(string eml, int enteredCode)
    {
        return await _APIService.Post<bool>(Endpoint.EmailVerify, new
        {
            email = eml,
            code = enteredCode
        });
    }
}
