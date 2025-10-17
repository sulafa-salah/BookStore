using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Contracts.Carts;
    public record AddItemRequest(string ProductId, string Name, decimal UnitPrice, int Quantity);