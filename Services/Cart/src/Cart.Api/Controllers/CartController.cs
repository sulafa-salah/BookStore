using Cart.Api.Extensions;
using Cart.Api.Mappings;
using Cart.Application.Carts.Commands.AddItem;
using Cart.Application.Carts.Commands.ClearCart;
using Cart.Application.Carts.Commands.RemoveItem;
using Cart.Application.Carts.Queries.GetCart;
using Cart.Contracts.Carts;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart.Api.Controllers;
[Authorize]
[Route("api/[controller]")]
   
    public class CartController : ApiController
    {
        private readonly IMediator _mediator;
        public CartController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return MissingUserIdProblem();

        var result = await _mediator.Send(new GetCartQuery(userId), ct);

        return result.Match(
            cart => Ok(ContractMapping.MapToCart(cart)),
            errors => Problem(errors)
        );
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequest request, CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return MissingUserIdProblem();

        var result = await _mediator.Send(
            new AddItemCommand(userId, request.ProductId, request.Name, request.UnitPrice, request.Quantity), ct);

        return result.Match(_ => NoContent(), errors => Problem(errors));
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItem(string productId, CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return MissingUserIdProblem();

        var result = await _mediator.Send(new RemoveItemCommand(userId, productId), ct);

        return result.Match(_ => NoContent(), errors => Problem(errors));
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return MissingUserIdProblem();

        var result = await _mediator.Send(new ClearCartCommand(userId), ct);

        return result.Match(_ => NoContent(), errors => Problem(errors));
    }

    private IActionResult MissingUserIdProblem()
       => Problem(new List<Error>
    {
        Error.Unauthorized(
            code: "Auth.UserIdMissing",
            description: "User id was not found in the access token.")
    });
}