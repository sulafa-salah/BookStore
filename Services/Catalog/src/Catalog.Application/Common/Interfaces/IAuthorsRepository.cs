using Catalog.Domain.AuthorAggregate;
using Catalog.Domain.CategoryAggreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Common.Interfaces;


public interface IAuthorsRepository
{
    Task AddAuthorAsync(Author author, CancellationToken ct);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);

    Task<Author?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<(IReadOnlyList<Author> Items, int TotalCount)> ListAuthorsAsync(
       int pageNumber, int pageSize,
       string? search, string? sortBy, bool desc,
       CancellationToken ct);

    Task<bool> ExistsByNameExcludingIdAsync(string name, Guid excludeId, CancellationToken ct);

    Task UpdateAsync(Author author);
}