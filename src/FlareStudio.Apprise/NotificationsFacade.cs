using FlareStudio.Apprise.Application;
using FlareStudio.Apprise.DataAccess;
using System;

namespace FlareStudio.Apprise
{
    public interface INotificationsFacade : IDisposable
    {
        void SendMessage(ITemplateModel templateModel, params string[] recipients);
        void StartMessageQueue();
    }

    public interface ITemplateModel
    {
        public string CshtmlTemplatePath { get; }
    }

    public interface ISmtpConfiguration
    {
        public string ServerHost { get; }
        public int ServerPort { get; }
        public string User { get; }
        public string Password { get; }
        public string SenderEmail { get; }
    }

    public interface ILoggerPort
    {
        void LogMessage(string message, params object[] values);
        void LogException(string message, Exception exception);
    }

    public class NotificationsFacade : INotificationsFacade
    {
        private readonly IBackgroundWorker _backgroundWorker;
        private readonly IMessageCreator _messageCreator;

        public NotificationsFacade(string connectionString, ISmtpConfiguration smtpConfiguration, ILoggerPort loggerPort = null)
        {
            loggerPort ??= new DefaultLoggerAdapter();

            var sqlConnectionResolver = new SqlConnectionResolver(connectionString);
            DbMigrator.Migrate(connectionString);

            var messageQueueProcessor = new MessageQueueProcessor(
                new DataAccess.Message.MessagesToProcessProvider(sqlConnectionResolver),
                new MessageRenderer(),
                new MessageSender(smtpConfiguration, loggerPort),
                new DataAccess.Message.MessageStateSaver(sqlConnectionResolver),
                loggerPort);

            _backgroundWorker = new BackgroundWorker(
                messageQueueProcessor,
                loggerPort);

            _messageCreator = new MessageCreator(
                new DataAccess.Message.MessageCreator(sqlConnectionResolver));
        }

        public void SendMessage(ITemplateModel templateModel, params string[] recipients)
        {
            _messageCreator.Create(templateModel, recipients);
        }

        public void StartMessageQueue()
        {
            _backgroundWorker.Start();
        }

        public void Dispose()
        {
            _backgroundWorker.Dispose();
        }
    }
}
