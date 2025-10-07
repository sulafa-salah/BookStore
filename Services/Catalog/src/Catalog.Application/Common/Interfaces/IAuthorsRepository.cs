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
}