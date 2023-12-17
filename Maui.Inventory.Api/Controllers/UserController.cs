using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Maui.Inventory.Api.Controllers;

[Route("api/user")]
public class UserController(IUserRepository userRepository) : BaseController()
{
    private readonly IUserRepository _UserRepository = userRepository;

    [HttpPost]
    [Route("register")]
    public async Task<APIResponse<UserResponse>> RegisterNewUser([FromBody] UserRegistration potentialNewUser)
    {
        return await _UserRepository.RegisterUser(
            potentialNewUser.AdminID,
            potentialNewUser.UserName,
            potentialNewUser.Password);
    }

    [HttpPost]
    [Route("login")]
    public async Task<APIResponse<AuthenticatedUser>> Login([FromBody] UserRegistration potentialExistingUser)
    {
        return await _UserRepository.AuthenticateUser(
            potentialExistingUser.UserName,
            potentialExistingUser.Password);
    }
}
