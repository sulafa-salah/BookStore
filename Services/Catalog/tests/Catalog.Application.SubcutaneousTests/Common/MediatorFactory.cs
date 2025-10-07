
using Catalog.Infrastructure.Persistence;
using Catalog.Api;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Catalog.Application.SubcutaneousTests.Common;

public class MediatorFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
{
    // Single in-memory SQLite connection reused inside this factory.
    // NOTE: With SQLite in-memory, the DB lives as long as the connection is open.
    private SqliteTestDatabase _testDatabase = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Create & initialize the in-memory database once per factory instance.
        _testDatabase = SqliteTestDatabase.CreateAndInitialize();

        // IMPORTANT:
        // - Remove any existing CatalogDbContext registration the API added.
        // - Re-register it to use our single open SQLite connection.
        // - RemoveAll<T> extension comes from Microsoft.Extensions.DependencyInjection.Extensions.

        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<DbContextOptions<CatalogDbContext>>()
                .AddDbContext<CatalogDbContext>((sp, options) => options.UseSqlite(_testDatabase.Connection));
        });
    }

    public IMediator CreateMediator()
    {
        // New service scope , clean DI lifetime for each test.
        var serviceScope = Services.CreateScope();

        // xUnit creates a NEW test class instance per test,  private readonly IMediator _mediator = factory.CreateMediator();

        // reset the database to guarantee isolation between tests.
        _testDatabase.ResetDatabase();

        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }


    public Task InitializeAsync() => Task.CompletedTask;

    public new Task DisposeAsync()
    {
        // Dispose our in-memory connection (drops the DB).
        _testDatabase.Dispose();

        return Task.CompletedTask;
    }
}