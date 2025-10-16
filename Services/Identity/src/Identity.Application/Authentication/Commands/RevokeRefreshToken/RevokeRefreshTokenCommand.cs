using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Authentication.Commands.RevokeRefreshToken;
    public  record RevokeRefreshTokenCommand(string RefreshToken)
       : IRequest<ErrorOr<Unit>>;