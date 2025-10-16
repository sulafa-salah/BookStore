using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using System.Security.Claims;
using Throw;

namespace Catalog.Api.Services;

    public class CurrentUserProvider(IHttpContextAccessor _httpContextAccessor) : ICurrentUserProvider
    {
        public CurrentUser GetCurrentUser()
        {
    
        var http = _httpContextAccessor.HttpContext;
        var principal = http?.User;
        if (principal?.Identity?.IsAuthenticated != true)
            return null;

        // Try common ids: sub or NameIdentifier
        var sub = principal.FindFirst("sub")?.Value
                  ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(sub, out var userId))
            return null; // don’t throw on bad token

        var email = principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? principal.FindFirst("email")?.Value;
        var id = GetClaimValues("id")
           .Select(Guid.Parse)
           .First();

        // Permissions could be multiple "permissions" claims or a CSV claim; support both.
        var roles = GetClaimValues(ClaimTypes.Role);


        return new CurrentUser(Id: id, Roles: roles);
    }

    private IReadOnlyList<string> GetClaimValues(string claimType)
    {
        return _httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();
    }
}
