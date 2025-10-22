using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor.Functions.Services;
    public sealed class ImageSharpThumbnailService(ILogger<ImageSharpThumbnailService> logger) : IThumbnailService
    {
        public async Task<string?> CreateThumbnailAsync(
            BlobContainerClient container, string sourceBlobName, string bookId, string outputExt = ".png",
            CancellationToken ct = default)
        {
            var src = container.GetBlobClient(sourceBlobName);
            if (!await src.ExistsAsync(ct))
            {
             //   logger.LogWarning("Source blob not found: {Blob}", sourceBlobName);
                return null; // permanent
            }

            var download = await src.DownloadContentAsync(ct);
            await using var inStream = download.Value.Content.ToStream();

            try
            {
                using var image = await Image.LoadAsync(inStream, ct);
                image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(300, 0), Mode = ResizeMode.Max }));

                var ext = outputExt.StartsWith('.') ? outputExt.ToLowerInvariant() : "." + outputExt.ToLowerInvariant();
                var thumbBlobName = $"thumbs/{bookId}{ext}";
                var thumb = container.GetBlobClient(thumbBlobName);

                await using var ms = new MemoryStream();
                if (ext is ".png") await image.SaveAsPngAsync(ms, ct);
                else await image.SaveAsJpegAsync(ms, ct);

                ms.Position = 0;
                await thumb.UploadAsync(ms, overwrite: true, cancellationToken: ct);

             //   logger.LogInformation("Thumbnail created at {Uri}", thumb.Uri);
                return thumbBlobName;
            }
            catch (UnknownImageFormatException uif)
            {
                logger.LogError(uif, "Unknown image format for {Blob}", sourceBlobName);
                return null; // permanent
            }
        }
    }