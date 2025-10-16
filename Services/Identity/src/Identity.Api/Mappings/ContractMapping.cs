using Identity.Application.Authentication.Common;
using Identity.Contracts.Authentication;

namespace Identity.Api.Mappings;
    public static class ContractMapping
    {
        public static AuthenticationResponse MapToAuthResponse(AuthenticationResult authResult)
    {
        return new AuthenticationResponse(
          
            authResult.User.Email,
            authResult.Token,
            authResult.RefreshToken,
            authResult.ExpiresAtUtc);
    }
}
    