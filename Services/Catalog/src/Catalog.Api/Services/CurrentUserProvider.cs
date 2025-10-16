using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using System.IdentityModel.Tokens.Jwt;
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


        var id = FindGuidClaim(principal, new[]
     {
            "id",
            JwtRegisteredClaimNames.Sub,
            ClaimTypes.NameIdentifier
        });

        if (id is null)
            return null; 

        var email = principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? principal.FindFirst("email")?.Value;
     
       
        var roles = GetClaimValues(ClaimTypes.Role);


        return new CurrentUser(Id: id.Value, Roles: roles);
    }
    private static Guid? FindGuidClaim(ClaimsPrincipal user, IEnumerable<string> claimTypes)
    {
        foreach (var type in claimTypes)
        {
            var val = user.FindFirst(type)?.Value;
            if (!string.IsNullOrWhiteSpace(val) && Guid.TryParse(val, out var g))
                return g;
        }
        return null;
    }
    private IReadOnlyList<string> GetClaimValues(string claimType)
    {
        return _httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();
    }
}
