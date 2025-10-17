using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Domain.Entities;
    public  class ShoppingCart
{
        public string UserId { get; set; } = default!;
        public List<CartItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);

        public void AddOrUpdateItem(string productId, string name, decimal unitPrice, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productId)) return;
            if (unitPrice < 0) unitPrice = 0;
            if (quantity <= 0) quantity = 1;

            var existing = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing is null)
            {
                Items.Add(new CartItem
                {
                    ProductId = productId,
                    Name = name,
                    UnitPrice = unitPrice,
                    Quantity = quantity
                });
            }
            else
            {
                existing.Quantity += quantity;
                existing.UnitPrice = unitPrice; 
            }
        }
    public void RemoveItem(string productId) => Items.RemoveAll(i => i.ProductId == productId);
    public void Clear() => Items.Clear();
}