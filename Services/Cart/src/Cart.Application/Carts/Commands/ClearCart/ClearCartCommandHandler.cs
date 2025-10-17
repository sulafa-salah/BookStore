using Cart.Application.Common.Interfaces;
using Cart.Domain.Entities;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Carts.Commands.ClearCart;
    public sealed class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, ErrorOr<Unit>>
    {
        private readonly ICartRepository _repo;
        public ClearCartCommandHandler(ICartRepository repo) => _repo = repo;

        public async Task<ErrorOr<Unit>> Handle(ClearCartCommand cmd, CancellationToken ct)
        {
            var existed = await _repo.RemoveAsync(cmd.UserId, ct);
            if (!existed) return CartErrors.NotFound(cmd.UserId);
            return Unit.Value;
        }
    }