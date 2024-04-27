﻿namespace PersonalPodcast.Services
{
    using System;
    using System.Net.Mail;

    public static class SecureMail
    {
        // SMTP server details
        private const string SmtpServer = "smtp-relay.brevo.com";
        private const int SmtpPort = 587;
        private const string SmtpUsername = "njnana2017@gmail.com";
        private const string SmtpPassword = "0x9DInWLh3F872U1";

        public static bool SendEmail(string fromAddress, string toAddress, string subject, string body)
        {
            MailMessage mail = new MailMessage(fromAddress, toAddress, subject, body);

            SmtpClient client = new SmtpClient(SmtpServer)
            {
                Port = SmtpPort,
                Credentials = new System.Net.NetworkCredential(SmtpUsername, SmtpPassword),
                EnableSsl = false
            };

            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

}
