using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Functions.Services;
    public interface IThumbnailService
    {
        /// Creates a 300px-wide (max) PNG or JPEG under thumbs/{bookId}.{ext}
        /// Returns the blob name (e.g., "thumbs/1234.png") or null if source invalid.
        Task<string?> CreateThumbnailAsync(
            BlobContainerClient container, string sourceBlobName, string bookId, string outputExt = ".png",
            CancellationToken ct = default);
    }