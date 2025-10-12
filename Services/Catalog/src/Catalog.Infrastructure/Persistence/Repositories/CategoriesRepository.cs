using Catalog.Application.Common.Interfaces;
using Catalog.Domain.CategoryAggreate;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;
   
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly CatalogDbContext _dbContext;

        public CategoriesRepository(CatalogDbContext dbContext) => _dbContext = dbContext;
       

        public async Task AddCategoryAsync(Category category, CancellationToken ct)
       
         =>   await _dbContext.Categories.AddAsync(category,ct);
       

        public Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
            => _dbContext.Categories.AnyAsync(c => c.Name == name, ct);


    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct) =>
    _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
    public async Task<(IReadOnlyList<Category> Items, int TotalCount)> ListCategoriesAsync(
         int pageNumber, int pageSize, string? search, string? sortBy, bool desc, CancellationToken ct)
    {
        var q = _dbContext.Categories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(c => c.Name.ToLower().Contains(s) || c.Description.ToLower().Contains(s));
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
   await _dbContext.Categories.AnyAsync(c => c.Id != excludeId && c.Name.ToLower() == name.ToLower(), ct);

    public Task UpdateAsync(Category category)
    {
        _dbContext.Update(category);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Categories
            .AnyAsync(c => c.Id == id, ct);
    }

}
