using Dapper;

namespace FlareStudio.Apprise.DataAccess.Message
{
    internal interface IMessageStateSaver
    {
        Domain.Message Save(Domain.Message message);
    }

    internal class MessageStateSaver : IMessageStateSaver
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public MessageStateSaver(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public Domain.Message Save(Domain.Message message)
        {
            const string sql = @"
UPDATE [Message]
SET [AttemptsPerformed] = @AttemptsPerformed,
    [MessageState] = @MessageState,
    [LastAttemptError] = @LastAttemptError,
    [LastAttemptTimestampUtc] = @LastAttemptTimestampUtc
OUTPUT inserted.*
WHERE [MessageId] = @MessageId";

            using(var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.QuerySingle<Domain.Message>(sql, message);
            }
        }
    }
}
