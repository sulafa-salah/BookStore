using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Common.Models;
    public sealed record BookDto(
      Guid Id,
      string Title,
      string Description,
      string Isbn,
      string Sku,
      decimal PriceAmount,
      string PriceCurrency,
      bool IsPublished,
      Guid CategoryId,
      IReadOnlyList<Guid> AuthorIds,
      string? CoverBlob,
      string? ThumbBlob,
      DateTime CreatedAt,
      DateTime? UpdatedAt
  );