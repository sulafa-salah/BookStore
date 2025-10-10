using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Contracts.Books;
    public  record CreateBookRequest(
    string Title,
    string Description,
    string Isbn,
    string Sku,
    decimal PriceAmount,
    string PriceCurrency,           
    Guid CategoryId,
    IReadOnlyList<Guid> AuthorIds   
);
