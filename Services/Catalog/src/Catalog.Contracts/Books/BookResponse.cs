using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Books;
    public  record BookResponse(
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
     DateTime CreatedAt,
     DateTime? UpdatedAt
 );