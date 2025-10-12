
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using Catalog.Domain.Common;
using Catalog.Infrastructure.IntegrationEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Catalog.Infrastructure.Persistence
{

   public class CatalogDbContext : DbContext, IUnitOfWork
       {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
        public DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents { get; set; } = null!;

        public CatalogDbContext(DbContextOptions options) : base(options)
        {
        }
           protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

        }

        

        public async Task CommitChangesAsync()
        {
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
            await SaveChangesAsync();
        }
    }
}
