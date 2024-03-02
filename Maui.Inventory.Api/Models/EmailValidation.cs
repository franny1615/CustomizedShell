namespace Maui.Inventory.Api.Models;

public class EmailValidation
{
    public int Id { get; set; } = -1;
    public string Email { get; set; } = string.Empty;
    public int Code { get; set; } = -1;
}

public class Feedback
{
    public int Id { get; set; } = 0;
    public int AdminId { get; set; } = 0;
    public int UserId { get; set; } = 0;
    public bool WasAdmin { get; set; } = false;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}