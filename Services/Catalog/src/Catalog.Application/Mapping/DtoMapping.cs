using Catalog.Application.Common.Models;
using Catalog.Domain.BookAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Mapping;

public static class DtoMapping
{
    public static BookDto MapToBookDto(this Book book)
    {
        return new BookDto(
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
            book.ThumbBlobName,
            book.CreatedAt,
            book.UpdatedAt
        );
    }
}

