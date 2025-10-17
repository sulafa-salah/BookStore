using ErrorOr;

namespace Cart.Domain.Entities;
    public static class CartErrors
    {
        public static Error NotFound(string userId) =>
            Error.NotFound(code: "Cart.NotFound", description: $"Cart for '{userId}' not found.");

        public static readonly Error Empty =
            Error.Validation(code: "Cart.Empty", description: "Cart is empty.");

        public static Error InvalidQuantity(int q) =>
            Error.Validation(code: "Item.Quantity", description: $"Quantity must be > 0. Got {q}.");

        public static Error InvalidPrice(decimal p) =>
            Error.Validation(code: "Item.UnitPrice", description: $"UnitPrice must be >= 0. Got {p}.");
    }