using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Maui.Inventory.Api.Controllers;

[Route("api/admin")]
public class AdminController(IUserRepository userRepository) : BaseController
{
    private readonly IUserRepository _UserRepository = userRepository;

    [HttpPost]
    [Route("register")]
    public async Task<APIResponse<UserResponse>> RegisterNewUser([FromBody] AdminRegistration potentialNewUser)
    {
        return await _UserRepository.RegisterAdmin(
            potentialNewUser.UserName,
            potentialNewUser.Password);
    }

    [HttpPost]
    [Route("login")]
    public async Task<APIResponse<AuthenticatedUser>> Login([FromBody] AdminRegistration potentialExistingUser)
    {
        return await _UserRepository.AuthenticateAdmin(
            potentialExistingUser.UserName,
            potentialExistingUser.Password);
    }
}
