using ErrorOr;
using Identity.Application.Authentication.Common;
using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Authentication.Commands.Register;
    public class RegisterCommandHandler :
      IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUsersRepository _usersRepository;

        public RegisterCommandHandler(  IJwtTokenGenerator jwtTokenGenerator, IRefreshTokenRepository refreshTokenRepository, IUsersRepository usersRepository)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _usersRepository = usersRepository;
        }

        public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
        if (await _usersRepository.ExistsByEmailAsync(command.Email)) return UserErrors.EmailAlreadyUsed;

        var create = ApplicationUser.Create(command.FirstName, command.LastName, command.Email);

        var user = create.Value;
       var userResult= await _usersRepository.AddUserAsync(user,command.Password);
        if (userResult.IsError) return userResult.Errors;


        // issue tokens
        var tokenResult = _jwtTokenGenerator.GenerateTokens(user);
        var tokenHash = _jwtTokenGenerator.Hash(tokenResult.RefreshToken);
        await _refreshTokenRepository.AddAsync(user.Id, tokenHash, cancellationToken);

        return new AuthenticationResult(user, tokenResult.AccessToken, tokenResult.RefreshToken, tokenResult.ExpiresAtUtc);
    }
    }