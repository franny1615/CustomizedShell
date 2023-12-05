using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Maui.Inventory.Api.Controllers;

[Route("api/v1/user")]
public class UserController : BaseController
{
    private readonly IUserRepository _UserRepository;

    public UserController(IUserRepository userRepository) : base()
    {
        _UserRepository = userRepository;
    }

    [HttpPost]
    [Route("register")]
    public async Task<APIResponse<string>> RegisterNewUser([FromBody] RegisterUser potentialNewUser)
    {
        bool success = await _UserRepository.RegisterNewUser(
            potentialNewUser.UserName,
            potentialNewUser.Password,
            potentialNewUser.IsAdmin);

        return new APIResponse<string> { Success = success, Data = "" };
    }

    [HttpPost]
    [Route("login")]
    public async Task<APIResponse<AuthenticatedUser>> Login([FromBody] RegisterUser potentialExistingUser)
    {
        AuthenticatedUser user = await _UserRepository.AuthenticateUser(
            potentialExistingUser.UserName,
            potentialExistingUser.Password);

        return new()
        {
            Success = !string.IsNullOrEmpty(user.AccessToken),
            Data = user
        };
    }

    [HttpGet]
    [Route("adminCheck")]
    public async Task<APIResponse<string>> AdminCheck()
    {
        bool adminExists = await _UserRepository.AdminCheck();

        return new() 
        {
            Success = adminExists, 
            Data = adminExists ? "admin exists": "admin NOT exists" 
        };
    }
}
