using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Carts.Commands.AddItem;
    public  record AddItemCommand(
      string UserId,
      string ProductId,
      string Name,
      decimal UnitPrice,
      int Quantity
  ) : IRequest<ErrorOr<Unit>>;