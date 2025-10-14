
using Catalog.Application.Common.Interfaces;

using Catalog.Infrastructure.IntegrationEvents.Settings;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;



namespace Catalog.Infrastructure;
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
        services
         .AddMediatR()
         .AddConfigurations(configuration)
   
         .AddPersistence(configuration)
          .AddMessaging(configuration);

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
      //  services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<CatalogDbContext>());
        return services;
    }
    // ---------- MassTransit  ---------- 
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var mq = new MessageBrokerSettings();
        configuration.Bind(MessageBrokerSettings.Section, mq);

        services.AddMassTransit(x =>
        {
        
            x.SetKebabCaseEndpointNameFormatter();
          
            x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
            {
                o.UseSqlServer();
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
        return services;
    }
}


