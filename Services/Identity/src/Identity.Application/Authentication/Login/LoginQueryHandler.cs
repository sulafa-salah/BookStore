using ErrorOr;
using Identity.Application.Authentication.Common;
using Identity.Application.Common.Interfaces;
using Identity.Domain.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Authentication.Login;
    public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUsersRepository _usersRepository;

        public LoginQueryHandler(
            IJwtTokenGenerator jwtTokenGenerator, IRefreshTokenRepository refreshTokenRepository,

            IUsersRepository usersRepository)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
           _usersRepository = usersRepository;
        }

        public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetByEmailAsync(query.Email);

        if (user is null) return AuthenticationErrors.InvalidCredentials;

        if (!await _usersRepository.CheckPasswordAsync(user, query.Password))
            return AuthenticationErrors.InvalidCredentials;
        // issue tokens
        var tokenResult = _jwtTokenGenerator.GenerateTokens(user);
        var tokenHash = _jwtTokenGenerator.Hash(tokenResult.RefreshToken);
        await _refreshTokenRepository.AddAsync(user.Id, tokenHash, cancellationToken);

        return new AuthenticationResult(user, tokenResult.AccessToken,tokenResult.RefreshToken, tokenResult.ExpiresAtUtc);
    }
    }