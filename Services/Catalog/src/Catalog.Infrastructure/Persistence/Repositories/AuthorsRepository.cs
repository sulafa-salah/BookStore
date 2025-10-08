using Catalog.Application.Common.Interfaces;
using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.CategoryAggreate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Persistence.Repositories;


    public class AuthorsRepository : IAuthorsRepository
{
        private readonly CatalogDbContext _dbContext;

        public AuthorsRepository(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    public async Task AddAuthorAsync(Author author, CancellationToken ct)

       => await _dbContext.Authors.AddAsync(author, ct);
    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
       => _dbContext.Authors.AnyAsync(c => c.Name == name, ct);

    public Task<Author?> GetByIdAsync(Guid id, CancellationToken ct) =>
   _dbContext.Authors.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
}

