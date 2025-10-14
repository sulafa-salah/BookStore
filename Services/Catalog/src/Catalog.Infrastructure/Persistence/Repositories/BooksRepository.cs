using Catalog.Application.Common.Interfaces;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Persistence.Repositories;
    
  public  class BooksRepository : IBooksRepository
    {
        private readonly CatalogDbContext _dbContext;

        public BooksRepository(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddBookAsync(Book book, CancellationToken ct = default)
        {
            await _dbContext.Books.AddAsync(book, ct);
        await _dbContext.SaveChangesAsync();
    }

        public async Task<bool> IsIsbnTakenAsync(string isbn, CancellationToken ct = default)
        {
            var value = ISBN.Create(isbn).Value;
            return await _dbContext.Books
                .AnyAsync(b => b.Isbn == value, ct);
        }

        public async Task<bool> IsSkuTakenAsync(string sku, CancellationToken ct = default)
        {
            var value = Sku.Create(sku).Value;
            return await _dbContext.Books
                .AnyAsync(b => b.Sku == value, ct);
        }
    }