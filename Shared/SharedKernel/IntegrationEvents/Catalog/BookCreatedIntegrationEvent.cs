using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.IntegrationEvents.Catalog;
    public record BookCreatedIntegrationEvent(string Name,Guid BookId,string ISBN) : IIntegrationEvent;
    

