using Maui.Inventory.Api.Controllers;
using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/email")]
public class EmailController(IUserRepository userRepository) : BaseController
{
    private readonly IUserRepository _userRepo = userRepository;

    [HttpPost]
    [Route("beginValidation")]
    public async Task<APIResponse<bool>> BeginValidation([FromBody] EmailValidation emailValidation)
    {
        return await _userRepo.BeginEmailVerification(
            emailValidation.Email
        );
    }

    [HttpPost]
    [Route("validate")]
    public async Task<APIResponse<bool>> ValidateEmail([FromBody] EmailValidation emailValidation)
    {
        return await _userRepo.VerifyEmail(
            emailValidation.Email,
            emailValidation.Code
        );
    }
}