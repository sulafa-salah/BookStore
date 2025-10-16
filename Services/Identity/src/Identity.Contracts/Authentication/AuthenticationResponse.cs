using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Contracts.Authentication;
    public record AuthenticationResponse(
       string Email,
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAtUtc
       );