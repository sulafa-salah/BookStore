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
       

        public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
        => _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
            => _dbContext.Categories.AnyAsync(c => c.Name == name, ct);

        public Task<bool> ExistsByNameExceptIdAsync(string name, Guid excludeId, CancellationToken ct)
            => _dbContext.Categories.AnyAsync(c => c.Name == name && c.Id != excludeId, ct);

        public async Task<IReadOnlyList<Category>> ListAsync(int page, int pageSize, bool? isActive, CancellationToken ct)
        {
            var query = _dbContext.Categories.AsQueryable();
            if (isActive is not null) query = query.Where(c => c.IsActive == isActive);

            return await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }
}
