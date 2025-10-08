using Catalog.Application.Common.Models;
using Catalog.Domain.AuthorAggregate;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Authors.Queries;

    public record ListAuthorsQuery(int PageNumber, int PageSize, string? Search, string? SortBy, string? SortDir) : IRequest<ErrorOr<PagedResult<Author>>>;