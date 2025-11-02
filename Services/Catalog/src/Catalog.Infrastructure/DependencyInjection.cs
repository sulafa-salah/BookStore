
using Azure.Storage.Blobs;
using Catalog.Application.Common.Interfaces;
using Catalog.Infrastructure.Authentication.TokenSetting;
using Catalog.Infrastructure.Caching;
using Catalog.Infrastructure.Caching.Redis;
using Catalog.Infrastructure.IntegrationEvents.Settings;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using Catalog.Infrastructure.Persistence.Storage.Azure;
using Catalog.Infrastructure.Queues;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;



namespace Catalog.Infrastructure;
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
        services
             .AddMediatR()
           .AddConfigurations(configuration)       
            .AddPersistence(configuration)         
            .AddMessaging(configuration, environment) 
            .AddStorage(configuration)              //  Blob Storage
            .AddCaching(configuration,environment)
            .AddJWTAuthentication(configuration);   // auth 
        return services;
    }
    public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.Section, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => 
            {

             
        options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero,
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx => { Console.WriteLine(ctx.Exception); return Task.CompletedTask; },
                    OnChallenge = ctx => { Console.WriteLine($"Challenge: {ctx.Error} {ctx.ErrorDescription}"); return Task.CompletedTask; }
                };
            });


        return services;
    }
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

        return services;
    }

 

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        var messageBrokerSettings = new MessageBrokerSettings();
        configuration.Bind(MessageBrokerSettings.Section, messageBrokerSettings);

        services.AddSingleton(Options.Create(messageBrokerSettings));

        // Azure Storage settings binding
        var storageSettings = new AzureStorageSettings();
        configuration.Bind(AzureStorageSettings.Section, storageSettings);
        services.AddSingleton(Options.Create(storageSettings));
        services.AddSingleton<IImageJobQueue, ImageJobQueue>();

        return services;
    }
    // ----------  Azure Blob Storage ----------
    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        // Prefer connection string under section, or ConnectionStrings:AzureStorage
        services.Configure<AzureStorageSettings>(
       configuration.GetSection(AzureStorageSettings.Section));

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<AzureStorageSettings>>().Value;
            return new BlobServiceClient(settings.ConnectionString);
        });
        services.AddScoped<IBlobStorage, AzureBlobStorage>();

        return services;
    }
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<CatalogDbContext>(options =>
 options.UseSqlServer(connectionString));

        services.AddScoped<IBooksRepository, BooksRepository>();
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<IAuthorsRepository, AuthorsRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CatalogDbContext>());
        return services;
    }
    // ---------- MassTransit  ---------- 
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var mq = new MessageBrokerSettings();
        configuration.Bind(MessageBrokerSettings.Section, mq);
        if (!environment.IsEnvironment("Testing"))
        { 
            services.AddMassTransit(x =>
        {

            x.SetKebabCaseEndpointNameFormatter();

            x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
            {
                o.UseSqlServer();
                o.DisableInboxCleanupService();
                o.UseBusOutbox();

            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(mq.HostName, configrator =>
                {
                    configrator.Username(mq.UserName);
                    configrator.Password(mq.Password);
                });
                cfg.UseMessageRetry(r =>
                {
                    r.Immediate(2);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
   }
        return services;
    }
    // ---------- Caching (FusionCache + Redis) ----------
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        if (environment.IsEnvironment("Testing"))
        {
            // register the in-memory cache for testing
            services.AddSingleton<ICacheService, InMemoryTestCacheService>();
            return services;
        }
        var redisSettings = configuration
     .GetSection("Redis")
     .Get<RedisOptions>()!;
        var redisConnection = redisSettings.ConnectionString;
       
        services
            .AddFusionCache() 
            .WithSerializer(
               new FusionCacheSystemTextJsonSerializer()
            )
            .WithDistributedCache(
                new RedisCache(new RedisCacheOptions
                {
                    Configuration = redisConnection
                })
            )
            .WithBackplane(
                new RedisBackplane(new RedisBackplaneOptions
                {
                    Configuration = redisConnection
                })
            )
            .WithDefaultEntryOptions(opts =>
            {
                // default cache policy
                opts.SetDuration(TimeSpan.FromMinutes(5));
                opts.SetFailSafe(true);
                //opts.SetFailSafeMaxDuration(TimeSpan.FromMinutes(30));
                //opts.SetJitterMaxDuration(TimeSpan.FromSeconds(10));
            });

        
        services.AddScoped<ICacheService, FusionCacheService>();

        return services;
    }
}


