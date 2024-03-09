using Maui.Inventory.Api.Controllers;
using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maui.Inventory.Api;

[Route("api/email")]
public class EmailController(
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository) : BaseController
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

    [HttpPost]
    [Authorize]
    [Route("feedback")]
    public async Task<APIResponse<bool>> SendFeedback([FromBody] Feedback feedbackEmail)
    {
        var response = new APIResponse<bool>();
        try
        {
            var user = httpContextAccessor.HttpContext?.User!;
            int adminId = Env.GetAdminIDFromIdentity(user);
            int userId = Env.GetUserIdFromIdentity(user);
            bool isAdmin = Env.IsAdmin(user);

            response = await _userRepo.InsertUserFeedback(feedbackEmail, userId, adminId, isAdmin);
        }
        catch (Exception ex)
        {
            response.Data = false;
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }

    [HttpGet]
    [Authorize]
    [Route("feedbackList")]
    public async Task<APIResponse<PaginatedQueryResponse<Feedback>>> GetFeedback([FromQuery] PaginatedRequest request)
    {
        var response = new APIResponse<PaginatedQueryResponse<Feedback>>();
        try
        {
            response = await _userRepo.GetFeedback(request);
        }
        catch (Exception ex)
        {
            response.Data = new();
            response.Success = false;
            response.Message = $"ERROR >>> {ex.Message} <<<";
        }

        return response;
    }

    [HttpPost]
    [Authorize]
    [Route("feedback/update")]
    public async Task<APIResponse<bool>> UpdateFeedback([FromBody] Feedback feedback)
    {
        return await _userRepo.UpdateFeedback(feedback);
    }

    [HttpPost]
    [Authorize]
    [Route("feedback/delete")]
    public async Task<APIResponse<bool>> DeleteFeedback([FromBody] Feedback feedback)
    {
        return await _userRepo.DeleteFeedback(feedback);
    }
}