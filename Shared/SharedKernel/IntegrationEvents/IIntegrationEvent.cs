using MediatR;
using SharedKernel.IntegrationEvents.Catalog;

using System.Text.Json.Serialization;


namespace SharedKernel.IntegrationEvents;

// Base interface for all integration events shared between microservices.

[JsonDerivedType(typeof(BookCreatedIntegrationEvent), typeDiscriminator: nameof(BookCreatedIntegrationEvent))]
public interface IIntegrationEvent : INotification { }