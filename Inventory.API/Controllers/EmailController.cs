using Inventory.API.Models;
using Inventory.API.Repositories.EmailRepository;
using Inventory.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[Route("api/email")]
public class EmailController(
    IHttpContextAccessor httpContextAccessor,
    IEmailRepository emailRepo) : BaseController(httpContextAccessor)
{
    [Route("beginValidation")]
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BeginValidation([FromBody] EmailValidation email)
    {
        var repoResult = await emailRepo.BeginEmailVerification(email.Email);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [Route("validate")]
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Validate(EmailValidation email)
    {
        var repoResult = await emailRepo.VerifyEmail(email.Email, email.Code);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }
}
