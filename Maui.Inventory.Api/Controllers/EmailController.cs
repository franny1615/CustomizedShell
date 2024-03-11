using Maui.Inventory.Api.Controllers;
using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maui.Inventory.Api;
using System.Globalization;

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

    [HttpGet]
    [Authorize]
    [Route("feedback/list/component")]
    public string GetFeedbackListComponent() => $@"
<div class=""container"" 
     hx-get=""/api/email/feedback/list/items?Page=0&ItemsPerPage=5""
     hx-trigger=""load delay:250ms""
     hx-target=""#feedback-list-items"">
    <div class=""container"" id=""feedback-list-items""></div>
</div>";

    [HttpGet]
    [Authorize]
    [Route("feedback/list/items")]
    public async Task<string> GetFeedbackListItems([FromQuery] PaginatedRequest request)
    {
        string response;
        
        try
        {
            var list = (await _userRepo.GetFeedback(request)).Data ?? new();

            var itemsStr = "";
            for (int i = 0; i < list.Items.Count; i++)
            {
                var item = list.Items[i];
                string completeStr = item.IsCompleted ?
                    "<span class=\"material-symbols-outlined\" style=\"color: green;\">check_circle</span>" :
                    "<span class=\"material-symbols-outlined\" style=\"color: red;\">cancel</span>";

                itemsStr += $@"
<tr>
    <td class=""table-dark"">{item.Subject}</td>
    <td class=""table-dark"">{item.Body}</td>
    <td class=""table-dark"">{item.UpdatedOn.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)}</td>
    <td class=""table-dark"">{item.CreatedOn.ToString("MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture)}</td>
    <td class=""table-dark"">{completeStr}</td>
</tr>";
            }

            var possiblePages = list.Total / request.ItemsPerPage;
            if (possiblePages % request.ItemsPerPage != 0)
            {
                possiblePages += 1;
            }
            int previous = request.Page == 0 ? 0 : request.Page - 1;
            int next = request.Page + 1 > possiblePages ? possiblePages : request.Page + 1;
            var pagination = $@"
<div class=""row mb-3 flex justify-content-center"">
    <ul class=""pagination flex justify-content-center"">
        <li class=""page-item"">
            <button class=""page-link"" 
               hx-get=""/api/email/feedback/list/items?Page={previous}&ItemsPerPage={request.ItemsPerPage}""
               hx-include=""#feedback-search""
               hx-trigger=""click""
               hx-target=""#feedback-list-items"">Previous</button>
        </li>
        <li class=""page-item"">
            <button class=""page-link""
               hx-get=""/api/email/feedback/list/items?Page={next}&ItemsPerPage={request.ItemsPerPage}""
               hx-include=""#feedback-search""
               hx-trigger=""click""
               hx-target=""#feedback-list-items"">Next</button>
        </li>
    </ul>
</div>";

            response = $@"
<div class=""row mb-3"">
    <div class=""input-group"">
        <span class=""input-group-text material-symbols-outlined"" id=""search-icon"">search</span>
        <input type=""text"" class=""form-control"" placeholder=""Search"" value=""{request.Search}"" aria-describedby=""search-icon"" id=""feedback-search"" name=""Search"">
    </div>
</div>
<div class=""row mb-3"">
    <table class=""table table-striped"">
        <thead>
            <tr>
                <th class=""table-dark"" scope=""col"">Subject</th>
                <th class=""table-dark"" scope=""col"">Body</th>
                <th class=""table-dark"" scope=""col"">Last Updated</th>
                <th class=""table-dark"" scope=""col"">Created On</th>
                <th class=""table-dark"" scope=""col"">Complete?</th>
            </tr>
        </thead>
        {itemsStr}
    </table>
</div>
{pagination}";
        }
        catch (Exception ex)
        {
            response = $"<h1>ERROR >>> {ex.Message} <<< /h1>";
        }

        return response;
    }
}