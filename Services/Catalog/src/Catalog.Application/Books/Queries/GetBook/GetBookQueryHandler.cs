using Catalog.Application.Common.Constants;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Application.Mapping;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.CategoryAggreate;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Queries.GetBook;
    public sealed class GetBookQueryHandler : IRequestHandler<GetBookQuery, ErrorOr<BookDto>>
    {
        private readonly IBooksRepository _booksRepository;
        private readonly ICacheService _cache;
    private readonly ILogger<GetBookQueryHandler> logger;

    public GetBookQueryHandler(IBooksRepository booksRepository, ICacheService cache, ILogger<GetBookQueryHandler> logger)
    {
        _booksRepository = booksRepository;
        _cache = cache;
        this.logger = logger;
    }

    public async Task<ErrorOr<BookDto>> Handle(GetBookQuery query, CancellationToken ct)
    {

        // 1. Build cache key
        var cacheKey = CacheKeys.Book(query.BookId);

        // 2. Try cache first; if miss, go to DB and store result
        var bookDto = await _cache.GetOrCreateAsync<BookDto?>(
            cacheKey,
            async token =>
            {
                logger.LogInformation($"Cache MISS for {cacheKey}");
                // this is the factory that will hit the DB
              
                  var entity = await _booksRepository.GetByIdAsync(query.BookId, token);
                  if (entity is null)
                      return null;

                  // map Domain -> Application DTO
                  return DtoMapping.MapToBookDto(entity);
              },
              ttl: TimeSpan.FromMinutes(5),
              ct
          );


                // 3. If still null => NotFound error
                return bookDto is null
                    ? BookErrors.NotFound
                    : bookDto;

            }
            }