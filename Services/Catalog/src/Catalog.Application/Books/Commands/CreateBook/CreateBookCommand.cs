using Catalog.Domain.BookAggregate;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Books.Commands.CreateBook;
    public  record CreateBookCommand(
    string Title,
    string Description,
    string Isbn,
    string Sku,
    decimal PriceAmount,
    string PriceCurrency,
    Guid CategoryId,
    IEnumerable<Guid> AuthorIds
) : IRequest<ErrorOr<Book>>;