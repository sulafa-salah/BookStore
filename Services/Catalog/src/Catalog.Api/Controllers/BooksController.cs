using Catalog.Application.Books.Commands.CreateBook;
using Catalog.Contracts.Books;
using MediatR;
using Catalog.Api.Mappings;
using Microsoft.AspNetCore.Mvc;

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
    }
}
