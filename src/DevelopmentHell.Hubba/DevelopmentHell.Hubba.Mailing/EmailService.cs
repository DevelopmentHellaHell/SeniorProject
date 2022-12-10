using DevelopmentHell.Hubba.Models;
using System.Net;
using System.Net.Mail;

namespace DevelopmentHell.Hubba.Emailing.Service
{
	public class EmailService
	{
		private static readonly string companyEmail = "noreply.Hubba@gmail.com"; // TODO: config
		private static readonly string companyPassword = "gdpayrbhauajfmok"; // TODO: config

		public static Result SendEmail(string email, string subject, string body)
		{
			SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
			{
				Credentials = new NetworkCredential(companyEmail, companyPassword),

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