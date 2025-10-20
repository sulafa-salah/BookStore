using Catalog.Api.Mappings;
using Catalog.Application.Books.Commands.CreateBook;
using Catalog.Application.Books.Commands.UpdateBookCover;
using Catalog.Application.Books.Queries.GetBook;
using Catalog.Application.Books.Queries.ListBooks;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Mappings;
using Catalog.Contracts.Books;
using Catalog.Infrastructure.Persistence.Storage.Azure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Catalog.Api.Controllers;
    [Route("api/[controller]")]
   
    public class BooksController : ApiController
    {
    private readonly ISender _mediator;
    private readonly IBlobStorage _blobStorage;
    private readonly IOptions<AzureStorageSettings> _storage;

    public BooksController(ISender mediator,
                          IBlobStorage blobStorage,
                          IOptions<AzureStorageSettings> storage)
    {
        _mediator = mediator;
        _blobStorage = blobStorage;
        _storage = storage;
    }

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
    

   [HttpPost("{bookId:guid}/cover")]
    public async Task<IActionResult> UploadCover(Guid bookId, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) return BadRequest("No file uploaded.");
        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only images allowed.");

        var container = _storage.Value.CoversContainer;          
        var ext = Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();

        var result = await _mediator.Send(new UpdateBookCoverCommand(
      bookId, stream, file.ContentType, ext, container), ct);

        return await result.MatchAsync<IActionResult>(
            async ok =>
            {
                var (id, blobName) = ok;
                var sas = _blobStorage.GetReadSasUri(container, blobName, TimeSpan.FromMinutes(30));
                return Ok(new { id, coverBlob = blobName, coverUrl = sas.ToString() });
            },
            errors => Task.FromResult(Problem(errors))
        );
    }
}


