using Catalog.Domain.BookAggregate.Events;
using Catalog.Domain.Common.ValueObjects;
using Catalog.Infrastructure.Persistence;
using MediatR;
using SharedKernel.IntegrationEvents;
using SharedKernel.IntegrationEvents.Catalog;
using System.Text.Json;


namespace Catalog.Infrastructure.IntegrationEvents.OutboxWriter;

    public class OutboxWriterEventHandler
        : INotificationHandler<BookCreatedEvent>
         

    {
        private readonly CatalogDbContext _dbContext;

        public OutboxWriterEventHandler(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new BookCreatedIntegrationEvent(
                Name: notification.Title,
                BookId: notification.BookId,
                ISBN : notification.Isbn);

            await AddOutboxIntegrationEventAsync(integrationEvent);
        }

      

        private async Task AddOutboxIntegrationEventAsync(IIntegrationEvent integrationEvent)
        {
            await _dbContext.OutboxIntegrationEvents.AddAsync(new OutboxIntegrationEvent(
                EventName: integrationEvent.GetType().Name,
                EventContent: JsonSerializer.Serialize(integrationEvent)));

            await _dbContext.SaveChangesAsync();
        }
    }
