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

public class BooksRepository : IBooksRepository
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

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken ct) =>
 await  _dbContext.Books.AsNoTracking().Include(b => b.BookAuthors).FirstOrDefaultAsync(c => c.Id == id, ct);
    public async Task<(IReadOnlyList<Book> Items, int TotalCount)> ListBooksAsync(
         int pageNumber, int pageSize, string? search, string? sortBy, bool desc, CancellationToken ct)
       
    {
        
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);
       
     
        IQueryable<Book> q =   _dbContext.Books
            .AsNoTracking()
            // MapToBook needs AuthorIds => include join table only (no heavy Author data)
            .Include(b => b.BookAuthors);

        // search 
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            q = q.Where(b =>
                EF.Functions.Like(b.Title, term) ||
                (b.Description != null && EF.Functions.Like(b.Description, term)) ||
                EF.Functions.Like(b.Isbn.Value, term) ||
                EF.Functions.Like(b.Sku.Value, term));
        }

        // sorting 
        q = (sortBy ?? "createdAt").ToLowerInvariant() switch
        {
            "title" => desc ? q.OrderByDescending(b => b.Title).ThenByDescending(b => b.Id)
                                 : q.OrderBy(b => b.Title).ThenBy(b => b.Id),
            "price" => desc ? q.OrderByDescending(b => b.Price.Amount).ThenByDescending(b => b.Id)
                                 : q.OrderBy(b => b.Price.Amount).ThenBy(b => b.Id),
            "createdat" or _
                         => desc ? q.OrderByDescending(b => b.CreatedAt).ThenByDescending(b => b.Id)
                                 : q.OrderBy(b => b.CreatedAt).ThenBy(b => b.Id),
        };

       
        var total = await q.CountAsync(ct);

        // page
        var items = await q
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);

    }

    public async void Update(Book book)
    {
        _dbContext.Books.Update(book);
        await _dbContext.SaveChangesAsync();
    }


}