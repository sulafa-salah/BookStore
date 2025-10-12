using Catalog.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.BookAggregate.Events;

  public  record BookCreatedEvent(Guid BookId,
    string Isbn,
    string Sku,
    decimal Price,
    string Currency,
    string Title,
    DateTime CreatedAtUtc
) : IDomainEvent;