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
    public async Task<APIResponse<UserResponse>> RegisterNewUser([FromBody] User potentialNewUser)
    {
        return await _UserRepository.RegisterUser(
            potentialNewUser.AdminID,
            potentialNewUser.UserName,
            potentialNewUser.Password,
            potentialNewUser.EditInventoryPermissions);
    }

    [HttpPost]
    [Route("login")]
    public async Task<APIResponse<User>> Login([FromBody] User potentialExistingUser)
    {
        return await _UserRepository.AuthenticateUser(
            potentialExistingUser.AdminID,
            potentialExistingUser.UserName,
            potentialExistingUser.Password);
    }
}
