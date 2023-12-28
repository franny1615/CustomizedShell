namespace Maui.Inventory;

public interface IEmailService
{
    public Task<bool> BeginVerification(string email);
    public Task<bool> Verify(string email, int enteredCode);
}
