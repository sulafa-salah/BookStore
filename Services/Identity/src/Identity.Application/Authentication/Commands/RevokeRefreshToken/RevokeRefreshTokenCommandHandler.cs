using ErrorOr;
using Identity.Application.Authentication.Common;
using Identity.Application.Common.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Authentication.Commands.RevokeRefreshToken;
  
   public sealed class RevokeRefreshTokenCommandHandler
    : IRequestHandler<RevokeRefreshTokenCommand, ErrorOr<Unit>>
    {
        private readonly IRefreshTokenRepository _refresh;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RevokeRefreshTokenCommandHandler(
            IRefreshTokenRepository refresh,
            IJwtTokenGenerator jwt)
        {
            _refresh = refresh;
        _jwtTokenGenerator = jwt;
        }

        public async Task<ErrorOr<Unit>> Handle(RevokeRefreshTokenCommand cmd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cmd.RefreshToken))
                return AuthenticationErrors.RefreshTokenInvalid;
        var hash = _jwtTokenGenerator.Hash(cmd.RefreshToken);
        var oldRefreshToken = await _refresh.FindByHashAsync(hash, ct);
        if (oldRefreshToken is null || oldRefreshToken.RevokedAt is not null)
            return AuthenticationErrors.RefreshTokenInvalid;
        await _refresh.RevokeAsync(oldRefreshToken.Id, ct);
         

            return Unit.Value;
        }
    }