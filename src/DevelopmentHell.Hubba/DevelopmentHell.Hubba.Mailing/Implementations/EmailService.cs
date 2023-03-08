using DevelopmentHell.Hubba.Email.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using System.Net;
using System.Net.Mail;

namespace DevelopmentHell.Hubba.Email.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private string _sendgridUsername;
        private string _sendgridApiKey;
		private string _companyEmail;
		public EmailService(string sendgridUsername, string sendgridApiKey, string companyEmail) {
            _sendgridUsername = sendgridUsername;
            _sendgridApiKey = sendgridApiKey;
			_companyEmail = companyEmail;
		}

        public Result SendEmail(string email, string subject, string body)
        {
            //string username = "apikey";
            //string companyEmail = "noreply.Hubba@gmail.com";

            SmtpClient client = new SmtpClient("smtp.sendgrid.net", 587)
            {
                Credentials = new NetworkCredential(_sendgridUsername, _sendgridApiKey),

                EnableSsl = true,
            };

            MailAddress to = new MailAddress(email);
            MailAddress from = new MailAddress(_companyEmail);

            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;

            Result result = new Result();
            try
            {
                client.Send(message);
                result.IsSuccessful = true;
                return result;
            }
            catch (SmtpException ex)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = ex.ToString();
                return result;
            }
        }
    }
}