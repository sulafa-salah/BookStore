using Cart.Application.Common.Authorization;
using Cart.Application.Common.Interfaces;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Common.Behaviors;
    public class AuthorizationBehavior<TRequest, TResponse>(ICurrentUserProvider _currentUserProvider)
       : IPipelineBehavior<TRequest, TResponse>
           where TRequest : IRequest<TResponse>
           where TResponse : IErrorOr
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var authorizationAttributes = request.GetType()
                .GetCustomAttributes<AuthorizeAttribute>()
                .ToList();

            if (authorizationAttributes.Count == 0)
            {
                return await next();
            }

            var currentUser = _currentUserProvider.GetCurrentUser();
            if (currentUser is null)
            {
                return (dynamic)Error.Unauthorized(description: "User is not authenticated");
            }
            var requiredRoles = authorizationAttributes
                    .SelectMany(authorizationAttribute => authorizationAttribute.Roles?.Split(',') ?? [])
                    .ToList();

            if (requiredRoles.Except(currentUser.Roles).Any())
            {
                return (dynamic)Error.Unauthorized(description: "User is forbidden from taking this action");
            }





            return await next();
        }
    }
