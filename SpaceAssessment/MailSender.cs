using SpaceAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAssessment
{
    internal static class MailSender
    {
        private static string SmtpServer = "smtp.office365.com";
        private static int Port = 587;
        private static bool UseSsl = true;
        public static bool SendReportMail(WheatherAnalysisResult report, string reportFilePath, string senderEmail, string password, string receiverEmail)
        {
            string subject = "Best spaceport and day";
            string body;
            if (report.BestSpaceport != null)
            {
                body = "The best combination of date and location for the space shuttle launch is: "
                    + $"{report.BestSpaceport.LocationName} on day {report.BestSpaceport.Day}.";
            }
            else
                body = "There is no suitable day for a space launch.";

            try
            {
                var smtpClient = new SmtpClient(SmtpServer, Port);
                smtpClient.EnableSsl = UseSsl;

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(senderEmail, password);

                var mailMessage = new MailMessage(senderEmail, receiverEmail, subject, body);

                if (File.Exists(reportFilePath))
                    mailMessage.Attachments.Add(new Attachment(reportFilePath));

                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
