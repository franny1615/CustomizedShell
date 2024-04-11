using Inventory.Api.Models;
using Inventory.Api.Repositories.UserRegistration;
using Inventory.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[Route("api/user")]
public class UserController(
    IUserRepository userRepo,
    IHttpContextAccessor httpContextAccessor): BaseController(httpContextAccessor)
{
    [HttpGet]
    [Route("check")]
    public async Task<bool> CheckUserName([FromQuery] string userName)
    {
        return await userRepo.DoesUserNameExist(userName);
    }

    [HttpPost]
    [Route("register")]
    public async Task<int> Register([FromBody] User user)
    {
        return await userRepo.CreateUser(user);
    }

    [HttpPost]
    [Route("login")]
    public async Task<string> Login([FromBody] User user)
    {
        return await userRepo.AuthenticateUser(user);
    }

    [HttpGet]
    [Authorize]
    [Route("details")]
    public async Task<User> GetDetails()
    {
        return await userRepo.GetUserById(UserId);
    }

    [HttpPost]
    [Authorize]
    [Route("update")]
    public async Task<bool> Update([FromBody] User user)
    {
        return await userRepo.UpdateUser(user);
    }

    [HttpPost]
    [Authorize]
    [Route("delete")]
    public async Task<bool> Delete([FromBody] User user)
    {
        return await userRepo.DeleteUser(user);
    }
}
