using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Catalog.Infrastructure.Persistence;



namespace Catalog.Api.IntegrationTests.Common;
/// <summary>
/// We're using SQLite so no need to spin an actual database.
/// </summary>
public class SqliteTestDatabase : IDisposable
{
    public SqliteConnection Connection { get; }

    public static SqliteTestDatabase CreateAndInitialize()
    {
        var testDatabase = new SqliteTestDatabase("DataSource=:memory:");

        testDatabase.InitializeDatabase();

        return testDatabase;
    }

    public void InitializeDatabase()
    {
        Connection.Open();
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlite(Connection)
            .Options;

        var context = new CatalogDbContext(options);

        context.Database.EnsureCreated();
    }

    public void ResetDatabase()
    {
        Connection.Close();

        InitializeDatabase();
    }

    private SqliteTestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }

    public void Dispose()
    {
        Connection.Close();
    }
}