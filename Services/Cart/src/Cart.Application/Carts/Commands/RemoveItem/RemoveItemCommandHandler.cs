using Cart.Application.Common.Interfaces;
using Cart.Domain.Entities;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Carts.Commands.RemoveItem;
    public sealed class RemoveItemCommandHandler : IRequestHandler<RemoveItemCommand, ErrorOr<Unit>>
    {
        private readonly ICartRepository _repo;
        public RemoveItemCommandHandler(ICartRepository repo) => _repo = repo;

        public async Task<ErrorOr<Unit>> Handle(RemoveItemCommand cmd, CancellationToken ct)
        {
            var cart = await _repo.GetAsync(cmd.UserId, ct);
            if (cart is null) return CartErrors.NotFound(cmd.UserId);

            cart.RemoveItem(cmd.ProductId);
            await _repo.SaveAsync(cart, ct: ct);
            return Unit.Value;
        }
    }