using Inventory.API.Models;

namespace Inventory.API.Repositories.EmailRepository;

public interface IEmailRepository
{
    public Task<RepoResult<bool>> BeginEmailVerification(string email);
    public Task<RepoResult<bool>> VerifyEmail(string email, int code);
}
