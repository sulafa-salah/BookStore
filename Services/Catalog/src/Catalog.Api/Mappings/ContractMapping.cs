using Catalog.Contracts.Books;
using Catalog.Domain.BookAggregate;
using Catalog.Domain.Common.ValueObjects;

namespace Catalog.Api.Mappings;

public static class ContractMapping
{
    public static BookResponse MapToBook(this Book book)
    {
        return new BookResponse(
            book.Id,
            book.Title,
            book.Description,
            book.Isbn.Value,
            book.Sku.Value,
            book.Price.Amount,
            book.Price.Currency,
            book.IsPublished,
            book.CategoryId,
            book.BookAuthors.Select(a => a.AuthorId).ToList(),
             book.CoverBlobName,
            CoverUrl: null,
            book.ThumbBlobName,
            ThumbUrl: null,
            book.CreatedAt,
            book.UpdatedAt
        );
    }
}