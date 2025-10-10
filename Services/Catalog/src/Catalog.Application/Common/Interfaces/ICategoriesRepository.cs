using Catalog.Domain.CategoryAggreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Common.Interfaces
{
    public interface ICategoriesRepository
    {
        Task AddCategoryAsync(Category category, CancellationToken ct);
       
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<(IReadOnlyList<Category> Items, int TotalCount)> ListCategoriesAsync(
         int pageNumber, int pageSize,
         string? search, string? sortBy, bool desc,
         CancellationToken ct);
        Task<bool> ExistsByNameExcludingIdAsync(string name, Guid excludeId, CancellationToken ct);

        Task UpdateAsync(Category category);


        Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct = default);

    }
}
