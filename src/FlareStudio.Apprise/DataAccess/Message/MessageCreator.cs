using Dapper;

namespace FlareStudio.Apprise.DataAccess.Message
{
    internal interface IMessageCreator
    {
        Domain.Message Create(MessageCreatorInput input);
    }

    internal class MessageCreatorInput
    {
        public string TemplateKey { get; set; }
        public string Model { get; set; }
        public string Recipients { get; set; }
    }

    internal class MessageCreator : IMessageCreator
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public MessageCreator(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public Domain.Message Create(MessageCreatorInput input)
        {
            const string sql = @"
INSERT INTO [Message] ([TemplateKey], [Model], [Recipients])
OUTPUT inserted.*
VALUES (@TemplateKey, @Model, @Recipients)";

            using(var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.QuerySingle<Domain.Message>(sql, input);
            }
        }
    }
}
