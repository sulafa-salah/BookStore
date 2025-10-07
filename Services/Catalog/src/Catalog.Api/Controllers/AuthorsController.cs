using Catalog.Application.Authors.Commands.CreateAuthor;
using Catalog.Contracts.Authors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [Route("api/[controller]")]
   
    public class AuthorsController(ISender _mediator) : ApiController
    {

        [HttpPost]
        public async Task<IActionResult> Create(CreateAuthorRequest request, CancellationToken ct)
       => (await _mediator.Send(new CreateAuthorCommand(request.Name, request.Bio), ct))
           .Match(a => Ok(new AuthorResponse(a.Id, a.Name, a.Biography,a.IsActive)), Problem);
    }
}
