using Identity.Application.Authentication.Common;
using Identity.Domain.UserAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Common.Interfaces;
    public interface IJwtTokenGenerator
    {
    TokenResult GenerateTokens(ApplicationUser user);
    string Hash(string value);
}