using ErrorOr;
using Identity.Api.Mappings;
using Identity.Application.Authentication.Commands.RefreshToken;
using Identity.Application.Authentication.Commands.Register;
using Identity.Application.Authentication.Commands.RevokeRefreshToken;
using Identity.Application.Authentication.Common;
using Identity.Application.Authentication.Login;
using Identity.Contracts.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthenticationController  : ApiController
{
    private readonly ISender _mediator;

    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.FirstName, request.LastName, request.Email, request.Password);
        ErrorOr<AuthenticationResult> authResult = await _mediator.Send(command);

        return authResult.Match(
            authResult => base.Ok(ContractMapping.MapToAuthResponse(authResult)),
            Problem);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);
        var authResult = await _mediator.Send(query);

        if (authResult.IsError && authResult.FirstError == AuthenticationErrors.InvalidCredentials)
        {
            return Problem(
                detail: authResult.FirstError.Description,
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return authResult.Match(
            authResult => Ok(ContractMapping.MapToAuthResponse(authResult)),
            Problem);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
   
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request)
    {
        var authResult = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));

        return authResult.Match(
            authResult => Ok(ContractMapping.MapToAuthResponse(authResult)),
            Problem);
    }

   
    [HttpPost("revoke")]
  
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RevokeAsync([FromBody] RevokeRefreshTokenRequest request)
    {
        var result = await _mediator.Send(new RevokeRefreshTokenCommand(request.RefreshToken));

        return result.Match(
            _ => NoContent(),
            Problem);
    }



}