using Catalog.Application.Common.Models;
using Catalog.Domain.BookAggregate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Queries.ListBooks;
   
    public sealed record ListBooksQuery(
        int PageNumber,
        int PageSize,
        string? Search,
        string? SortBy,
        string? SortDir
    ) : IRequest<ErrorOr<PagedResult<Book>>>;