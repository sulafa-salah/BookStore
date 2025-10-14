using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Queries.ListBooks;
   
  public  class ListBooksQueryHandler : IRequestHandler<ListBooksQuery, ErrorOr<PagedResult<Book>>>
    {
        private readonly IBooksRepository _booksRepository;

        public ListBooksQueryHandler(IBooksRepository booksRepository) => _booksRepository = booksRepository;

        public async Task<ErrorOr<PagedResult<Book>>> Handle(ListBooksQuery query, CancellationToken ct)
        {
            var listBooks = await _booksRepository.ListBooksAsync(
                query.PageNumber, query.PageSize, query.Search, query.SortBy, query.SortDir.Equals("desc", StringComparison.OrdinalIgnoreCase), ct);

            return new PagedResult<Book>(listBooks.Items, listBooks.TotalCount, query.PageNumber, query.PageSize);
      
  
}
    }