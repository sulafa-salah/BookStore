using Catalog.Application.Books.Commands.CreateBook;
using Catalog.Contracts.Books;
using MediatR;
using Catalog.Api.Mappings;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Books.Queries.ListBooks;
using Catalog.Application.Common.Mappings;
using Catalog.Application.Books.Queries.GetBook;

namespace Catalog.Api.Controllers
{
    [Route("api/[controller]")]
   
    public class BooksController(ISender _mediator) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest request, CancellationToken ct)
        {
            var command = new CreateBookCommand(
                request.Title,
                request.Description,
                request.Isbn,
                request.Sku,
                request.PriceAmount,
                request.PriceCurrency,
                request.CategoryId,
                request.AuthorIds 
            );

            var createBookResult = await _mediator.Send(command, ct);

            return createBookResult.Match(
              book => Ok(book.MapToBook()),
    Problem
);
        }

     [HttpGet("{bookId:guid}")]
public async Task<IActionResult> GetById(Guid bookId, CancellationToken ct)
{
    var q = new GetBookQuery(bookId);
    var result = await _mediator.Send(q, ct);

    return result.Match(
        b => Ok(b.MapToBook()),
        errors => Problem(errors)
    );
}

[HttpGet]
public async Task<IActionResult> List([FromQuery] GetBooksRequest req, CancellationToken ct)
    => (await _mediator.Send(
        new ListBooksQuery(req.PageNumber, req.PageSize, req.Search, req.SortBy, req.SortDir),
        ct))
    .Match(
        paged => Ok(paged.ToResponse(b => b.MapToBook())),
        errors => Problem(errors)
    );
    }
}
