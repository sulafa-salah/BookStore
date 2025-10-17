using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Carts.Commands.RemoveItem;
    public  record RemoveItemCommand(string UserId, string ProductId) : IRequest<ErrorOr<Unit>>;