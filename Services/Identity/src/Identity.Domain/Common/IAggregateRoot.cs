using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Common;
    public interface IAggregateRoot 
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        
        List<IDomainEvent> PopDomainEvents();
    }