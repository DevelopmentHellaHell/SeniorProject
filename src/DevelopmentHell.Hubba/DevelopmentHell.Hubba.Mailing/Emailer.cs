using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace DevelopmentHell.Hubba.Mailing
{
    public class Emailer
    {
        public static bool SendEmail(string email, string subject, string body)
        {
            MailAddress to = new MailAddress(email); //enter user's email address here
            MailAddress from = new MailAddress(ConfigurationManager.AppSettings["HubbaEmailAddress"]!);
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject; //title of email
            message.Body = body; //write body of email here
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(ConfigurationManager.AppSettings["HubbaEmailAddress"], ConfigurationManager.AppSettings["HubbaEmailPassword"]),
                EnableSsl = true
            };
            try
            {
                client.Send(message);
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}