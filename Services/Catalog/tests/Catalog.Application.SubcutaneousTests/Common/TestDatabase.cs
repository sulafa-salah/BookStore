using Catalog.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite; 
using Microsoft.EntityFrameworkCore;


namespace Catalog.Application.SubcutaneousTests.Common;


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

        // If DbContext has more ctor deps, supply them here.
        var context = new CatalogDbContext(options);

        // EnsureCreated  for test schemas (no migrations needed).
        context.Database.EnsureCreated();
    }

    public void ResetDatabase()
    {
        // Closing the connection drops the in-memory DB.
        Connection.Close();
        // Re-create a brand new empty database/schema.
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