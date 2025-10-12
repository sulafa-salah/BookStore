using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.IntegrationEvents;
    public record OutboxIntegrationEvent(string EventName, string EventContent);