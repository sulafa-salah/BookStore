using Cart.Contracts.Carts;
using Cart.Domain.Entities;

namespace Cart.Api.Mappings;
    public static class ContractMapping
    {
        public static CartResponse MapToCart(this ShoppingCart cart)
        {
            return new CartResponse(
                cart.UserId,
            cart.Total,
            cart.Items.Select(i => new CartItemResponse(i.ProductId, i.Name, i.UnitPrice, i.Quantity)).ToList()
       
            );
        }
    }