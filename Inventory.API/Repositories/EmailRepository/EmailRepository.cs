using System.Net.Mail;
using System.Net;
using System.Text;
using Inventory.API.Models;

namespace Inventory.API.Repositories.EmailRepository;

public class EmailRepository : BaseRepository, IEmailRepository
{
    public async Task<RepoResult<bool>> BeginEmailVerification(string email)
    {
        int generatedNumber = new Random().Next(1000, 9999);

        #region EMAIL BOILER PLATE
        MailMessage mail = new();
        mail.To.Add(email);
        mail.From = new MailAddress(Env.BusinessEmail, "Verify Email", Encoding.UTF8);
        mail.Subject = "Verify Email";
        mail.SubjectEncoding = Encoding.UTF8;
        mail.Body = $"<h1>Your verification code is : <b>{generatedNumber}</b> </h1>";
        mail.BodyEncoding = Encoding.UTF8;
        mail.IsBodyHtml = true;
        mail.Priority = MailPriority.High;

        SmtpClient client = new();
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(Env.BusinessEmail, Env.BusinessEmailPassword);
        client.EnableSsl = true;
        client.Port = 587;
        client.Host = "smtp.gmail.com";
        #endregion

        var response = new RepoResult<bool>();
        try
        {
            client.Send(mail);

            #region QUERY
            string query = $@"
            INSERT INTO email_validation
            (
                Email,
                Code
            )
            VALUES
            (
                '{email}',
                {generatedNumber}
            )";

            await QueryAsync<object>(query);
            #endregion

            response.Data = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.ToString();
        }

        return response;
    }

    public async Task<RepoResult<bool>> VerifyEmail(string email, int code)
    {
        var response = new RepoResult<bool>();
        try
        {
            #region QUERY
            string query = $@"
            SELECT Email, Code, Id 
            FROM email_validation
            WHERE Email = '{email}'
            AND Code = {code}";
            #endregion

            var emailValidation = (await QueryAsync<EmailValidation>(query)).FirstOrDefault();

            if (emailValidation == null)
            {
                response.Data = false;
            }
            else
            {
                // it is temporary so clean it up,
                // keep table slim
                await QueryAsync<object>($@"
                    DELETE FROM email_validation 
                    WHERE Id = {emailValidation.Id}");

                response.Data = true;
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage = $"ERROR >>> {ex.Message} <<<";
        }
        return response;
    }
}
