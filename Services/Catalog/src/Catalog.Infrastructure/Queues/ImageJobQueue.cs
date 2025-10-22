using Azure.Storage.Queues;
using Catalog.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;


namespace Catalog.Infrastructure.Queues;
    public class ImageJobQueue : IImageJobQueue
    {
        private readonly QueueClient _queue;

        public ImageJobQueue(IConfiguration config)
        {
            var connectionString = config["AzureWebJobsStorage"]
                ?? throw new InvalidOperationException("Missing AzureWebJobsStorage configuration.");

            _queue = new QueueClient(
                connectionString,
                "image-jobs",
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            _queue.CreateIfNotExists();
        }

        public async Task EnqueueAsync(Guid bookId, string blobName, string contentType, CancellationToken ct)
        {
            var payload = JsonSerializer.Serialize(new
            {
                BookId = bookId.ToString(),
                BlobName = blobName,
                ContentType = contentType
            });

            await _queue.SendMessageAsync(payload, cancellationToken: ct);
        }
    }
