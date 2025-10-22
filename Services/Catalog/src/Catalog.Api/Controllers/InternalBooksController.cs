using Catalog.Application.Books.Commands.UpdateBookThumbnail;
using Catalog.Contracts.Books;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("internal/books")]
[ApiExplorerSettings(IgnoreApi = true)]
public class InternalBooksController : ControllerBase
{
    private readonly IMediator _mediator;
    public InternalBooksController(IMediator mediator) => _mediator = mediator;

    [HttpPut("{id:guid}/thumbnail")]
   
    public async Task<IActionResult> SetThumbnail(Guid id, [FromBody] UpdateBookThumbnailRequest dto)
    {
        var result = await _mediator.Send(new UpdateBookThumbnailCommand(id, dto.ThumbBlobName));
        return result.IsError ? Problem(detail: result.FirstError.Description, statusCode: 400) : NoContent();
    }
}