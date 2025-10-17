using Cart.Application.Common.Interfaces;
using Cart.Domain.Entities;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Carts.Commands.AddItem;
    public  class AddItemCommandHandler : IRequestHandler<AddItemCommand, ErrorOr<Unit>>
    {
        private readonly ICartRepository _repo;
        public AddItemCommandHandler(ICartRepository repo) => _repo = repo;

        public async Task<ErrorOr<Unit>> Handle(AddItemCommand cmd, CancellationToken ct)
        {
            var cart = await _repo.GetAsync(cmd.UserId, ct) ?? new ShoppingCart { UserId = cmd.UserId };
            cart.AddOrUpdateItem(cmd.ProductId, cmd.Name, cmd.UnitPrice, cmd.Quantity);
            await _repo.SaveAsync(cart, ct: ct);
            return Unit.Value;
        }
    }