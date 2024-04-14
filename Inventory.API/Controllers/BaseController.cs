using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
public class BaseController(IHttpContextAccessor httpContextAccessor)
{
    public int UserId => Int("Id");
    public int CompanyId => Int("CompanyID");
    public bool IsCompanyOwner => Bool("IsCompanyOwner");
    public string UserName => String("UserName");
    public string Email => String("Email");

    private string String(string key)
    {
        ClaimsPrincipal? user = httpContextAccessor?.HttpContext?.User;
        string? claim = user?.Claims?.FirstOrDefault((c) => c.Type == key)?.Value;
        return claim ?? "";
    } 

    private int Int(string key)
    {
        return int.TryParse(String(key), out int id) ? id : -1;
    }

    private bool Bool(string key)
    {
        return String(key) == "true";
    }
}
