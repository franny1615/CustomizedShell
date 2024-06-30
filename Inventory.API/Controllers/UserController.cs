using Inventory.API.Models;
using Inventory.API.Repositories.UserRegistration;
using Inventory.API.Controllers;
using Inventory.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inventory.API.Repositories;

namespace Inventory.API.Controllers;

[Route("api/user")]
public class UserController(
    IUserRepository userRepo,
    IHttpContextAccessor httpContextAccessor): BaseController(httpContextAccessor)
{
    [HttpGet]
    [Route("check")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckUserName([FromQuery] string userName)
    {
        var repoResult = await userRepo.DoesUserNameExist(userName);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        var repoResult = await userRepo.CreateUser(user);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] User user)
    {
        var repoResult = await userRepo.AuthenticateUser(user);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse($"\"{repoResult.Data}\"");
    }

    [HttpGet]
    [Authorize]
    [Route("details")]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDetails()
    {
        var repoResult = await userRepo.GetUserById(UserId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpPost]
    [Authorize]
    [Route("update")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody] User user)
    {
        var repoResult = await userRepo.UpdateUser(user);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpDelete]
    [Authorize]
    [Route("delete")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromBody] User user)
    {
        var repoResult = await userRepo.DeleteUser(user);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }

    [HttpGet]
    [Authorize]
    [Route("search")]
    [ProducesResponseType<SearchResult<User>>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchStatuses([FromQuery] SearchRequest request)
    {
        var repoResult = await userRepo.Get(request, CompanyId);
        if (!string.IsNullOrEmpty(repoResult.ErrorMessage))
        {
            return Resp.ErrorRespose(repoResult.ErrorMessage);
        }
        return Resp.OkResponse(repoResult.Data);
    }
}
