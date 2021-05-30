using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace FlareStudio.Apprise.Application
{
    internal interface IMessageSender
    {
        SendResult Send(string subject, string body, string recipients);
    }

    internal class SendResult
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
    }

    internal class MessageSender : IMessageSender
    {
        private readonly ISmtpConfiguration _smtpConfiguration;
        private readonly ILoggerPort _loggerPort;

        public MessageSender(ISmtpConfiguration smtpConfiguration, ILoggerPort loggerPort)
        {
            _smtpConfiguration = smtpConfiguration;
            _loggerPort = loggerPort;
        }

        public SendResult Send(string subject, string body, string recipients)
        {
            _loggerPort.LogMessage("Sending e-mail to {0} with subject: {1}.", recipients, subject);

            if (string.IsNullOrEmpty(_smtpConfiguration.ServerHost))
            {
                _loggerPort.LogMessage("E-mail configuration is missing. Failing...");

                return new SendResult { Exception = new Exception("E-mail configuration is missing") };
            }

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            try
            {
                using (var email = new MailMessage(
                    _smtpConfiguration.SenderEmail,
                    recipients,
                    subject,
                    body))
                {
                    email.IsBodyHtml = true;

                    using (var smtpClient = new SmtpClient(
                        _smtpConfiguration.ServerHost,
                        _smtpConfiguration.ServerPort))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.EnableSsl = true;

                        smtpClient.Credentials = new NetworkCredential(
                             _smtpConfiguration.User,
                             _smtpConfiguration.Password);

                        smtpClient.Send(email);

                        return new SendResult { Success = true };
                    }
                }
            }
            catch (Exception exception)
            {
                return new SendResult { Exception = exception };
            }
        }
    }
}
