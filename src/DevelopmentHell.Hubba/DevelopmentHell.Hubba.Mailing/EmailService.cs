using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace DevelopmentHell.Hubba.Mailing
{
    public class EmailService
    {
        public static bool SendEmail(string email, string subject, string body)
        {
            MailAddress to = new MailAddress(email); //enter user's email address here
            MailAddress from = new MailAddress("noreply.Hubba@gmail.com"); // TODO: config
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject; //title of email
            message.Body = body; //write body of email here
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("noreply.Hubba@gmail.com", "gdpayrbhauajfmok"), // TODO: config

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