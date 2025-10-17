using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Contracts.Carts;
    public  record CartResponse(string UserId, decimal Total, IReadOnlyList<CartItemResponse> Items);