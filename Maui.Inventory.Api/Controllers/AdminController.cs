using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet]
    [Route("users")]
    [Authorize]
    public async Task<APIResponse<PaginatedQueryResponse<UserRegistration>>> GetAllUsers([FromQuery] UsersRequest request)
    {
        return await _UserRepository.GetUsersForAdmin(request);
    }

    [HttpPost]
    [Route("deleteUser")]
    [Authorize]
    public async Task<APIResponse<bool>> DeleteUser([FromBody] UserRegistration user)
    {
        return await _UserRepository.DeleteUserForAdmin(user.AdminID, user.Id);
    }

    [HttpPost]
    [Route("updateUser")]
    [Authorize]
    public async Task<APIResponse<bool>> UpdateUser([FromBody] UserRegistration user)
    {
        return await _UserRepository.EditUser(user);
    }
}
