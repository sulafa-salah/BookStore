
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using Catalog.Domain.Common;
using Catalog.Infrastructure.IntegrationEvents;
using MassTransit;
using MassTransit.Mediator;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Catalog.Infrastructure.Persistence;

   public class CatalogDbContext : DbContext
       {
     
        private readonly IPublisher _publisher;
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
     

        public CatalogDbContext(DbContextOptions options, IPublisher publisher) : base(options)
        {
        _publisher = publisher;
    }
           protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddInboxStateEntity();
    }

        

       
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
        // 1) Auditing
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = null;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
        // 2) pop domain events BEFORE saving
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
               .Select(entry => entry.Entity.PopDomainEvents())
               .SelectMany(x => x)
               .ToList();


        // 3) publish domain events (handlers call IPublishEndpoint.Publish)
        if (domainEvents.Count > 0)
                   {
                       foreach (var de in domainEvents)
                await _publisher.Publish(de, cancellationToken);
                  }
        return await base.SaveChangesAsync(cancellationToken);
        }

      
     

    
    }