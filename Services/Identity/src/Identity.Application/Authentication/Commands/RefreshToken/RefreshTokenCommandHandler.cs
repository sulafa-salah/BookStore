using ErrorOr;
using Identity.Application.Authentication.Common;
using Identity.Application.Common.Interfaces;
using MediatR;

namespace Identity.Application.Authentication.Commands.RefreshToken;
  
   public  class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthenticationResult>>
    {
        private readonly IRefreshTokenRepository _refresh;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUsersRepository _usersRepository;

    public RefreshTokenCommandHandler(IRefreshTokenRepository refresh, IJwtTokenGenerator jwtTokenGenerator, IUsersRepository usersRepository)
        {
            _refresh = refresh;
        _jwtTokenGenerator = jwtTokenGenerator;
        _usersRepository = usersRepository;
    }

        public async Task<ErrorOr<AuthenticationResult>> Handle(RefreshTokenCommand cmd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cmd.RefreshToken))
                return AuthenticationErrors.RefreshTokenInvalid;

            var hash = _jwtTokenGenerator.Hash(cmd.RefreshToken);
            var oldRefreshToken = await _refresh.FindByHashAsync(hash, ct);

            if (oldRefreshToken is null || oldRefreshToken.RevokedAt is not null)
                return AuthenticationErrors.RefreshTokenInvalid;

            if (DateTime.UtcNow > oldRefreshToken.ExpiresAt)
                return AuthenticationErrors.RefreshTokenExpired;
        var user = await _usersRepository.GetByIdAsync(oldRefreshToken.UserId);
        if(user is null) 
                return AuthenticationErrors.UserNotFound;


        if (!user.IsActive) return AuthenticationErrors.UserDeactivated;
        // Revoke current refresh token and generate new one
        await _refresh.RevokeAsync(oldRefreshToken.Id, ct);

        var tokenResult = _jwtTokenGenerator.GenerateTokens(user);
        var tokenHash = _jwtTokenGenerator.Hash(tokenResult.RefreshToken);
        await _refresh.AddAsync(user.Id, tokenHash, ct);

        return new AuthenticationResult(user, tokenResult.AccessToken, tokenResult.RefreshToken, tokenResult.ExpiresAtUtc);
    }
}

