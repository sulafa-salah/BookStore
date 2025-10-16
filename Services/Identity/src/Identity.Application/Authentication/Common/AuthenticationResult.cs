using Identity.Domain.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Authentication.Common;
    public record AuthenticationResult(
        ApplicationUser User,
        string Token,
        string RefreshToken,
         DateTime ExpiresAtUtc);