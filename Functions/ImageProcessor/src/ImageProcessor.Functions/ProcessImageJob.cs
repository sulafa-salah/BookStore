using Azure.Storage.Blobs;
using Google.Protobuf.WellKnownTypes;
using ImageProcessor.Functions.Contracts;
using ImageProcessor.Functions.Options;
using ImageProcessor.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net.Http.Json;
using System.Text.Json;

namespace ImageProcessor.Functions;
public class ProcessImageJob
{
    private readonly ILogger<ProcessImageJob> _logger;
    private readonly BlobServiceClient _blobs;
    private readonly IThumbnailService _thumbnail;
    private readonly ICatalogInternalClient _catalog;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IImageJobDeserializer _deserializer;
    private readonly ProcessorOptions _opts;
    public ProcessImageJob(ILogger<ProcessImageJob> logger,
        BlobServiceClient blobs, IThumbnailService thumbnail, ICatalogInternalClient catalog,
        IHttpClientFactory httpFactory, IImageJobDeserializer deserializer, IOptions<ProcessorOptions> options)
    {
        _logger = logger;
        _blobs = blobs;
        _thumbnail = thumbnail;
        _catalog = catalog;
        _httpFactory = httpFactory;
        _deserializer = deserializer;
        _opts = options.Value;
    }

    [Function("ProcessImageJob")]
    public async Task RunAsync(
         [QueueTrigger("image-jobs", Connection = "AzureWebJobsStorage")] string raw,
         CancellationToken ct)
    {
        // 1) Deserialize (permanent failure => swallow, don't retry)
        ImageJob? job = _deserializer.TryDeserialize(raw);
        if (job is null || string.IsNullOrWhiteSpace(job.BookId) || string.IsNullOrWhiteSpace(job.BlobName))
        {
            _logger.LogWarning("Invalid queue message, skipping. Raw={Raw}", raw);
            return;
        }

        // 2) Storage container
        var container = new BlobContainerClient(_opts.AzureWebJobsStorage, _opts.MediaContainer);
        await container.CreateIfNotExistsAsync(cancellationToken: ct);

        // 3) Create thumbnail (permanent failures are swallowed)
        var thumbBlobName = await _thumbnail.CreateThumbnailAsync(container, job.BlobName, job.BookId, ".png", ct);
        if (thumbBlobName is null) return;

        // 4) Update Catalog — transient failure => throw to retry
        var updated = await _catalog.UpdateThumbnailAsync(job.BookId, thumbBlobName, ct);
        if (!updated)
        {
            _logger.LogError("Failed to update catalog for BookId={BookId}", job.BookId);
            // Let Functions retry per host.json policy
            throw new InvalidOperationException("Catalog update failed; retrying.");
        }

        _logger.LogInformation("Processed image job for BookId={BookId}", job.BookId);
    }
}