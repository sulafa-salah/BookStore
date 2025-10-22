using Azure.Storage.Queues;
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
using Microsoft.AspNetCore.Authorization;
using Catalog.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Catalog.Api.Controllers;
    [Route("api/[controller]")]
   
    public class BooksController : ApiController
    {
    private readonly ISender _mediator;
    private readonly IBlobStorage _blobStorage;
    private readonly IOptions<AzureStorageSettings> _storage;
    private readonly IImageJobQueue _queue;

    public BooksController(IMediator mediator, IBlobStorage blobStorage, IOptions<AzureStorageSettings> storage,  IImageJobQueue queue)
    {
        _mediator = mediator;
        _blobStorage = blobStorage;
        _storage = storage;
        _queue = queue;
    }
    [HttpPost]
    [AllowAnonymous]
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
        var result = await _mediator.Send(new GetBookQuery(bookId), ct);

        return result.Match(
            b =>
            {
                var dto = b.MapToBook();

                var coverUrl = _blobStorage.TryGetSasUrl(_storage.Value.CoversContainer, dto.CoverBlob);
                var thumbUrl = _blobStorage.TryGetSasUrl(_storage.Value.ThumbsContainer, dto.ThumbBlob);

                // records are immutable; use `with`
                var enriched = dto with { CoverUrl = coverUrl, ThumbUrl = thumbUrl };
                return Ok(enriched);
            },
            errors => Problem(errors)
        );
    }

    [HttpGet]
   
    public async Task<IActionResult> List([FromQuery] GetBooksRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ListBooksQuery(req.PageNumber, req.PageSize, req.Search, req.SortBy, req.SortDir),
            ct);

        return result.Match(
            paged =>
            {
                var enriched = paged.Items
                    .Select(b =>
                    {
                        var dto = b.MapToBook();
                        return dto with
                        {
                            CoverUrl = _blobStorage.TryGetSasUrl(_storage.Value.CoversContainer, dto.CoverBlob),
                            ThumbUrl = _blobStorage.TryGetSasUrl(_storage.Value.ThumbsContainer, dto.ThumbBlob)
                        };
                    });

                return Ok(paged.ToResponse(_ => enriched)); 
            },
            errors => Problem(errors)
        );
    }

    [HttpPost("{bookId:guid}/cover")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadCover(Guid bookId, IFormFile file, CancellationToken ct)
    {
        // 1- Validate input — ensure a file was uploaded and it's an image type.
        if (file is null || file.Length == 0) return BadRequest("No file uploaded.");
        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only images allowed.");

        //  2. Extract the file extension (.jpg, .png, etc.)
        var ext = Path.GetExtension(file.FileName);

        // 3. Open the file stream for reading.
        await using var stream = file.OpenReadStream();

        //  4. Send the command to the Application layer to handle the upload.
        // The command encapsulates the business logic for updating the book cover.
        var result = await _mediator.Send(
            new UpdateBookCoverCommand(bookId, stream, file.ContentType, ext, _storage.Value.CoversContainer), ct);
        //  5. Handle the result using the ErrorOr pattern 
        return await result.MatchAsync<IActionResult>(
            async ok =>
            {
                // ok = (bookId, blobName) returned from the command handler
                var (id, blobName) = ok;
                // 6. Generate a short-lived SAS URL so the client can access the uploaded image securely.
                var sas = _blobStorage.GetReadSasUri(_storage.Value.CoversContainer, blobName, TimeSpan.FromMinutes(30));

                //  7. Enqueue a background job for image thumbnail generation.
                // This sends a lightweight message to Azure Queue Storage.
                await _queue.EnqueueAsync(id, blobName, file.ContentType, ct);
                //  8. Return the book ID, blob name, and temporary cover URL to the client.
                return Ok(new { id, coverBlob = blobName, coverUrl = sas.ToString() });
            },
            //  9. If the command failed, translate the domain/application errors to HTTP ProblemDetails.
            errors => Task.FromResult(Problem(errors))
        );
    }
}




