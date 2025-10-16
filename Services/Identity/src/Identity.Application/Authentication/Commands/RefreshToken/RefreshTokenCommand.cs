using ErrorOr;
using Identity.Application.Authentication.Common;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Authentication.Commands.RefreshToken;
    public  record RefreshTokenCommand(string RefreshToken)
     : IRequest<ErrorOr<AuthenticationResult>>;