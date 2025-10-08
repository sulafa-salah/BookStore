
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Domain.AuthorAggregate;
using ErrorOr;
using MediatR;


namespace Catalog.Application.Authors.Queries.ListAuthors;

   
     public class ListAuthorsQueryHandler : IRequestHandler<ListAuthorsQuery, ErrorOr<PagedResult<Author>>>
    {
        private readonly IAuthorsRepository _authorsRepository;
    public ListAuthorsQueryHandler(IAuthorsRepository authorsRepository) => _authorsRepository = authorsRepository;
        

    public async Task<ErrorOr<PagedResult<Author>>> Handle(ListAuthorsQuery r, CancellationToken ct)
        {
            var listAuthors = await _authorsRepository.ListAuthorsAsync(
                  r.PageNumber, r.PageSize, r.Search, r.SortBy, r.SortDir.Equals("desc", StringComparison.OrdinalIgnoreCase), ct);
            return new PagedResult<Author>(listAuthors.Items, listAuthors.TotalCount, r.PageNumber, r.PageSize);
        }
    }