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

}
