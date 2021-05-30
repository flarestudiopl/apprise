using DbUp;
using System;

namespace FlareStudio.Apprise.DataAccess
{
    internal static class DbMigrator
    {
        public static void Migrate(string connectionString)
        {
            var migrator = DeployChanges
                .To
                .SqlDatabase(connectionString)
                .JournalToSqlTable("dbo", "__NotificationsMigrationsHistory")
                .WithScriptsEmbeddedInAssembly(typeof(DbMigrator).Assembly)
                .LogToConsole()
                .Build();

            var migratorResult = migrator.PerformUpgrade();

            if (!migratorResult.Successful)
            {
                throw new Exception($"Error when executing '{migratorResult.ErrorScript?.Name}'", migratorResult.Error);
            }
        }
    }
}
