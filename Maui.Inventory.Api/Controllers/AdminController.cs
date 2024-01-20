using Maui.Inventory.Api.Interfaces;
using Maui.Inventory.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maui.Inventory.Api.Controllers;

[Route("api/admin")]
public class AdminController(
    IUserRepository userRepository, 
    IHttpContextAccessor httpContextAccessor) : BaseController
{
    private readonly IUserRepository _UserRepository = userRepository;
    private readonly IHttpContextAccessor _HttpContextAccessor = httpContextAccessor;

    [HttpPost]
    [Route("register")]
    public async Task<APIResponse<UserResponse>> RegisterNewUser([FromBody] Admin potentialNewUser)
    {
        return await _UserRepository.RegisterAdmin(
            potentialNewUser.UserName,
            potentialNewUser.Password,
            potentialNewUser.Email,
            potentialNewUser.EmailVerified);
    }

    [HttpPost]
    [Route("login")]
    public async Task<APIResponse<Admin>> Login([FromBody] Admin potentialExistingUser)
    {
        return await _UserRepository.AuthenticateAdmin(
            potentialExistingUser.UserName,
            potentialExistingUser.Password);
    }

    [HttpGet]
    [Route("users")]
    [Authorize]
    public async Task<APIResponse<PaginatedQueryResponse<User>>> GetAllUsers([FromQuery] PaginatedRequest paginatedRequest)
    {
        APIResponse<PaginatedQueryResponse<User>> response;

        try
        {
            var user = _HttpContextAccessor.HttpContext?.User!;
            int adminId = Env.GetAdminIDFromIdentity(user);

            response = await _UserRepository.GetUsersForAdmin(paginatedRequest, adminId);
        }
        catch (Exception ex) 
        {
            response = new();
            response.Success = false;
            response.Message = ex.Message;
            response.Data = new();
        }

        return response;
    }

    [HttpPost]
    [Route("deleteUser")]
    [Authorize]
    public async Task<APIResponse<bool>> DeleteUser([FromBody] User user)
    {
        return await _UserRepository.DeleteUserForAdmin(user.AdminID, user.Id);
    }

    [HttpPost]
    [Route("updateUser")]
    [Authorize]
    public async Task<APIResponse<bool>> UpdateUser([FromBody] User user)
    {
        return await _UserRepository.EditUser(user);
    }

    [HttpPost]
    [Route("updateAdmin")]
    [Authorize]
    public async Task<APIResponse<bool>> UpdateAdmin([FromBody] Admin admin)
    {
        return await _UserRepository.EditAdmin(admin);
    }
}
