using Catalog.Application.Common.Interfaces;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Queries.GetBook;
    public sealed class GetBookQueryHandler : IRequestHandler<GetBookQuery, ErrorOr<Book>>
    {
        private readonly IBooksRepository _booksRepository;

        public GetBookQueryHandler(IBooksRepository booksRepository) => _booksRepository = booksRepository;

        public async Task<ErrorOr<Book>> Handle(GetBookQuery query, CancellationToken ct)
        {
           
        var book = await _booksRepository.GetByIdAsync(query.BookId, ct);
        return book is null
            ? BookErrors.NotFound
            : book;
    }
    }