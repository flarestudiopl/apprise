using Newtonsoft.Json;
using FlareStudio.Apprise.DataAccess.Message;
using FlareStudio.Apprise.Domain;
using System;

namespace FlareStudio.Apprise.Application
{
    internal interface IMessageQueueProcessor
    {
        void Process();
    }

    internal class MessageQueueProcessor : IMessageQueueProcessor
    {
        const byte MAX_ATTEMPTS_PER_MESSAGE = 3;

        private readonly IMessagesToProcessProvider _messagesToProcessProvider;
        private readonly IMessageRenderer _messageRenderer;
        private readonly IMessageSender _messageSender;
        private readonly IMessageStateSaver _messageStateSaver;
        private readonly ILoggerPort _loggerPort;

        public MessageQueueProcessor(
            IMessagesToProcessProvider messagesToProcessProvider,
            IMessageRenderer messageRenderer,
            IMessageSender messageSender,
            IMessageStateSaver messageStateSaver,
            ILoggerPort loggerPort)
        {
            _messagesToProcessProvider = messagesToProcessProvider;
            _messageRenderer = messageRenderer;
            _messageSender = messageSender;
            _messageStateSaver = messageStateSaver;
            _loggerPort = loggerPort;
        }

        public void Process()
        {
            var messagesToProcess = _messagesToProcessProvider.Provide();

            foreach (var messageToProcess in messagesToProcess)
            {
                var templateType = Type.GetType(messageToProcess.TemplateKey); // TODO - cache

                _loggerPort.LogMessage("Processing {2} message (Id={0}) to {1}.", messageToProcess.MessageId, messageToProcess.Recipients, templateType.Name);

                var model = JsonConvert.DeserializeObject(messageToProcess.Model, templateType);
                var renderedMessage = _messageRenderer.Render(((ITemplateModel)model).CshtmlTemplatePath, model);
                var sendResult = _messageSender.Send(renderedMessage.Subject, renderedMessage.HtmlBody, messageToProcess.Recipients);
                SetResult(messageToProcess, sendResult);
            }
        }

        private void SetResult(Message messageToProcess, SendResult sendResult)
        {
            if (sendResult.Success)
            {
                messageToProcess.MessageState = MessageState.Sent;
            }
            else if (messageToProcess.AttemptsPerformed < MAX_ATTEMPTS_PER_MESSAGE)
            {
                messageToProcess.MessageState = MessageState.Retrying;
            }
            else
            {
                messageToProcess.MessageState = MessageState.Failed;
            }

            messageToProcess.AttemptsPerformed++;
            messageToProcess.LastAttemptTimestampUtc = DateTime.UtcNow;
            messageToProcess.LastAttemptError = sendResult.Exception?.ToString();

            _messageStateSaver.Save(messageToProcess);
        }
    }
}
