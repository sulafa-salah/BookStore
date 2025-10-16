using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Authentication.Common;
    public record TokenResult(
     string AccessToken,
     string RefreshToken,
     DateTime ExpiresAtUtc
 );