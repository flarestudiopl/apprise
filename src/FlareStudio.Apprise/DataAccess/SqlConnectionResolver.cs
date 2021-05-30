using System.Data.Common;
using System.Data.SqlClient;

namespace FlareStudio.Apprise.DataAccess
{
    internal interface ISqlConnectionResolver
    {
        DbConnection Resolve();
    }

    internal class SqlConnectionResolver : ISqlConnectionResolver
    {
        private readonly string _connectionString;

        public SqlConnectionResolver(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbConnection Resolve()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
