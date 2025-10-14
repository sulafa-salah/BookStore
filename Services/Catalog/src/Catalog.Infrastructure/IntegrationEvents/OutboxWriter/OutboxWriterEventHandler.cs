using Catalog.Domain.BookAggregate.Events;
using Catalog.Domain.Common.ValueObjects;
using Catalog.Infrastructure.Persistence;
using MassTransit;
using MassTransit.Transports;
using MediatR;
using SharedKernel.IntegrationEvents.Catalog;
using System.Text.Json;


namespace Catalog.Infrastructure.IntegrationEvents.OutboxWriter;

    public class OutboxWriterEventHandler
        : INotificationHandler<BookCreatedEvent>
         

    {
     
    private readonly IPublishEndpoint _publishEndpoint;
    public OutboxWriterEventHandler(IPublishEndpoint publish)
        => _publishEndpoint = publish;

    public async Task Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
        {
           await _publishEndpoint.Publish(new BookCreatedIntegrationEvent(
                Name: notification.Title,
                BookId: notification.BookId,
                ISBN : notification.Isbn));

          
        }
      
        }
    
