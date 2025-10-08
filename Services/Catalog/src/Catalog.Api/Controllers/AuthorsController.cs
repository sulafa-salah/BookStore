using Catalog.Application.Authors.Commands.CreateAuthor;
using Catalog.Application.Authors.Queries.GetAuthor;
using Catalog.Contracts.Authors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

    [Route("api/[controller]")]
   
    public class AuthorsController(ISender _mediator) : ApiController
    {

        [HttpPost]
        public async Task<IActionResult> Create(CreateAuthorRequest request, CancellationToken ct)
       => (await _mediator.Send(new CreateAuthorCommand(request.Name, request.Bio), ct))
           .Match(a => Ok(new AuthorResponse(a.Id, a.Name, a.Biography,a.IsActive)), Problem);
    

      [HttpGet("{authorId:guid}")]
        public async Task<IActionResult> GetById(Guid authorId, CancellationToken ct)
        {
            var q = new GetAuthorQuery(authorId);
            var result = await _mediator.Send(q, ct);

            return result.Match(
                a => Ok(new AuthorResponse(a.Id, a.Name, a.Biography, a.IsActive)),
                errors => Problem(errors)
            );
        }
    }
