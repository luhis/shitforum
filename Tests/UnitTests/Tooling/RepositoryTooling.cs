using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace UnitTests.Tooling
{
    public static class RepositoryTooling
    {
        public static void RunInConnection<T>(Func<ForumContext, T> createRepo, Action<T> f)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ForumContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new ForumContext(options))
                {
                    context.SeedData().Wait();
                }

                // Insert seed data into the database using one instance of the context
                using (var context = new ForumContext(options))
                {
                    var rep = createRepo(context);
                    f(rep);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
