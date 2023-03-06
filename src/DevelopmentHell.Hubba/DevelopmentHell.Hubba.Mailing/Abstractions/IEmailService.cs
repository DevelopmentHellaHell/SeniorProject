using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.Email.Service.Abstractions
{
    public interface IEmailService
    {
        Result SendEmail(string email, string subject, string body);
    }
}
