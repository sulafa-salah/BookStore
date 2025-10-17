using Cart.Application.Common.Interfaces;
using Cart.Domain.Entities;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Carts.Queries.GetCart;
    public sealed class GetCartQueryHandler : IRequestHandler<GetCartQuery, ErrorOr<ShoppingCart>>
    {
        private readonly ICartRepository _repo;
        public GetCartQueryHandler(ICartRepository repo) => _repo = repo;

        public async Task<ErrorOr<ShoppingCart>> Handle(GetCartQuery req, CancellationToken ct)
            => await _repo.GetAsync(req.UserId, ct) ?? new ShoppingCart { UserId = req.UserId };
    }