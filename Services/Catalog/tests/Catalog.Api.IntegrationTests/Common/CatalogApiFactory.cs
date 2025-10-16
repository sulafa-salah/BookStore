using Catalog.Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;


namespace Catalog.Api.IntegrationTests.Common;

    public class CatalogApiFactory : WebApplicationFactory<IAssemblyMarker>, IAsyncLifetime
    {
        private SqliteTestDatabase _testDatabase = null!;

        public HttpClient HttpClient { get; private set; } = null!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "test-only-super-long-256-bit-secret-change-me",
                ["JwtSettings:Issuer"] = "IdentityBookStore",
                ["JwtSettings:Audience"] = "BookStore",
            });
        });
        _testDatabase = SqliteTestDatabase.CreateAndInitialize();
     

        builder.ConfigureTestServices(services =>
            {
                services
                    .RemoveAll<DbContextOptions<CatalogDbContext>>()
                    .AddDbContext<CatalogDbContext>((sp, options) => options.UseSqlite(_testDatabase.Connection));

                // Remove RabbitMQ registrations and MassTransit HostedService if any
                services.RemoveAll<IBus>();
                services.RemoveAll<IBusControl>();
                services.RemoveAll<IHostedService>();

                // Use in-memory MassTransit Test Harness
                services.AddMassTransitTestHarness(cfg =>
                {
                    cfg.UsingInMemory((context, cfgInMemory) =>
                    {
                        cfgInMemory.ConfigureEndpoints(context);
                    });
                });
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = "Test";
                    o.DefaultChallengeScheme = "Test";
                })
       .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            });
        }

        public Task InitializeAsync()
        {
            HttpClient = CreateClient();

            return Task.CompletedTask;
        }

        public new Task DisposeAsync()
        {
            _testDatabase.Dispose();

            return Task.CompletedTask;
        }

        public void ResetDatabase()
        {
            _testDatabase.ResetDatabase();
        }
    }