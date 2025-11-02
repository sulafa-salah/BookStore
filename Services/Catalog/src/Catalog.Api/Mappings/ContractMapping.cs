using Catalog.Application.Common.Models;
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
    public static BookResponse MapToBookResponse(
           this BookDto dto,
           string? coverUrl,
           string? thumbUrl)
    {
        return new BookResponse(
            dto.Id,
            dto.Title,
            dto.Description,
            dto.Isbn,
            dto.Sku,
            dto.PriceAmount,
            dto.PriceCurrency,
            dto.IsPublished,
            dto.CategoryId,
            dto.AuthorIds,
            dto.CoverBlob,
            coverUrl,
            dto.ThumbBlob,
            thumbUrl,
            dto.CreatedAt,
            dto.UpdatedAt
        );
    }
}