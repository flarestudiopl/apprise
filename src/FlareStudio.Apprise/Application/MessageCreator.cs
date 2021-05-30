using FlareStudio.Apprise.DataAccess.Message;
using System.Text.Json;

namespace FlareStudio.Apprise.Application
{
    internal interface IMessageCreator
    {
        void Create(ITemplateModel templateModel, string[] recipients);
    }

    internal class MessageCreator : IMessageCreator
    {
        private readonly DataAccess.Message.IMessageCreator _messageQueueCreator;

        public MessageCreator(DataAccess.Message.IMessageCreator messageQueueCreator)
        {
            _messageQueueCreator = messageQueueCreator;
        }

        public void Create(ITemplateModel templateModel, string[] recipients)
        {
            if (recipients.Length == 0)
            {
                return;
            }

            var templateKey = templateModel.GetType().AssemblyQualifiedName;

            var creatorInput = new MessageCreatorInput
            {
                TemplateKey = templateKey,
                Model = JsonSerializer.Serialize<object>(
                    templateModel,
                    new JsonSerializerOptions
                    {
                        IgnoreReadOnlyProperties = true
                    }),
                Recipients = string.Join(',', recipients)
            };

            _messageQueueCreator.Create(creatorInput);
        }
    }
}
