using DevelopmentHell.Hubba.Models;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace DevelopmentHell.Hubba.Emailing.Service
{
	public class EmailService
	{
		public static Result SendEmail(string email, string subject, string body, bool enabledSend)
		{
			string username = ConfigurationManager.AppSettings["SENDGRID_USERNAME"]!;
			string companyEmail = ConfigurationManager.AppSettings["COMPANY_EMAIL"]!;
			SmtpClient client = new SmtpClient("smtp.sendgrid.net", 587)
			{
				Credentials = new NetworkCredential(username, ConfigurationManager.AppSettings["SENDGRID_API_KEY"]),

				EnableSsl = true,
			};

			MailAddress to = new MailAddress(email);
			MailAddress from = new MailAddress(companyEmail);

			MailMessage message = new MailMessage(from, to);
			message.Subject = subject;
			message.Body = body;

			Result result = new Result();
			try
			{
				if (enabledSend) client.Send(message);
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