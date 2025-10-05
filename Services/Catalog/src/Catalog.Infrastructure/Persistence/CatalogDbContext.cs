
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;


namespace Catalog.Infrastructure.Persistence
{

   public class CatalogDbContext : DbContext, IUnitOfWork
       {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();

        public CatalogDbContext(DbContextOptions options) : base(options)
        {
        }
           protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

           
        }

        

        public async Task CommitChangesAsync()
        {
            await SaveChangesAsync();
        }
    }
}
