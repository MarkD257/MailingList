
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;

namespace WFLibrary
{
	public class SendMail
	{
		public static int? Port { get; set; }
		public static string? Server { get; set; }

		public static bool Send(string source, string destination, string subject, string message, string? cc = null,
			string? bcc = null, System.Net.NetworkCredential? credentials = null, params Attachment[] attachments)
		{
			try
			{
				MailMessage mail = new MailMessage(source, destination);
				SmtpClient client = new SmtpClient();
				//client.Port = Port ?? Properties.Settings.Default.SMTPPort;

				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.UseDefaultCredentials = false;

				if (credentials != null)
				{
					client.Credentials = credentials;
				}

				//client.Host = Server ?? Properties.Settings.Default.SMTPServer;
				mail.Subject = subject;
				mail.Body = message;


				foreach (Attachment attachment in attachments) mail.Attachments.Add(attachment);


				if (cc != null)
					mail.CC.Add(cc);

				if (bcc != null)
					mail.Bcc.Add(bcc);
				//client.Send(mail);
				return true;

			}
			catch (Exception ex)
			{
				//Note - this is a bad solution.  It should write to the event log.  But we need an updated installer to create the inital event log entry, since inital event log access requires admin privilges.  
				try
				{
					File.WriteAllText(Path.Combine(Path.GetTempPath(), "Pisceas.NET" + Path.GetTempFileName()), "Cannot send email for subject " + subject + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace + Environment.NewLine + "Email Message:" + Environment.NewLine + message);
				}
				catch (Exception ex2)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message + Environment.NewLine + ex2.Message);
				}
				return false;
			}
		}
	}
}
