using Dapper;
using FlareStudio.Apprise.Domain;
using System.Collections.Generic;

namespace FlareStudio.Apprise.DataAccess.Message
{
    internal interface IMessagesToProcessProvider
    {
        IList<Domain.Message> Provide();
    }

    internal class MessagesToProcessProvider : IMessagesToProcessProvider
    {
        private readonly ISqlConnectionResolver _sqlConnectionResolver;

        public MessagesToProcessProvider(ISqlConnectionResolver sqlConnectionResolver)
        {
            _sqlConnectionResolver = sqlConnectionResolver;
        }

        public IList<Domain.Message> Provide()
        {
            const string sql = @"
SELECT TOP(10) *
FROM [Message]
WHERE [MessageState] IN @FetchStates";

            using (var connection = _sqlConnectionResolver.Resolve())
            {
                return connection.Query<Domain.Message>(
                                    sql,
                                    new
                                    {
                                        FetchStates = new[] { MessageState.Enqueued, MessageState.Retrying }
                                    })
                                 .AsList();
            }
        }
    }
}
