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

    public async Task<(IReadOnlyList<Author> Items, int TotalCount)> ListAuthorsAsync(
      int pageNumber, int pageSize, string? search, string? sortBy, bool desc, CancellationToken ct)
    {
        var q = _dbContext.Authors.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(c => c.Name.ToLower().Contains(s) || c.Biography.ToLower().Contains(s));
        }

        // sort
        q = (sortBy?.ToLower()) switch
        {
            "name" => desc ? q.OrderByDescending(c => c.Name) : q.OrderBy(c => c.Name),
            "createdat" => desc ? q.OrderByDescending(c => c.CreatedAt) : q.OrderBy(c => c.CreatedAt),
            _ => desc ? q.OrderByDescending(c => c.Name) : q.OrderBy(c => c.Name)
        };

        var total = await q.CountAsync(ct);
        var items = await q.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, total);
    }

    public async Task<bool> ExistsByNameExcludingIdAsync(string name, Guid excludeId, CancellationToken ct) =>
   await _dbContext.Authors.AnyAsync(c => c.Id != excludeId && c.Name.ToLower() == name.ToLower(), ct);

    public Task UpdateAsync(Author author)
    {
        _dbContext.Update(author);

        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Authors
            .AnyAsync(a => a.Id == id, ct);
    }

    public async Task<IReadOnlyList<Guid>> GetMissingIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var existingIds = await _dbContext.Authors
            .Where(a => ids.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync(ct);

        var missing = ids.Except(existingIds).ToList();
        return missing;
    }
}

