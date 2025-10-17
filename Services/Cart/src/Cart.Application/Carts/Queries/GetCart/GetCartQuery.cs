using Cart.Domain.Entities;
using ErrorOr;
using MediatR;


namespace Cart.Application.Carts.Queries.GetCart;
public  record GetCartQuery(string UserId) : IRequest<ErrorOr<ShoppingCart>>;