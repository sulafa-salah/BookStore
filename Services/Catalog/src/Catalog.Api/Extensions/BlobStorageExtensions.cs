using Catalog.Application.Common.Interfaces;

namespace Catalog.Api.Extensions;
    public static class BlobStorageExtensions
    {
        /// <summary>
        /// Generates a short-lived SAS URL for a blob if the blob name is provided.
        /// Returns null if the blob is missing or invalid.
        /// </summary>
        public static string? TryGetSasUrl(
            this IBlobStorage blobStorage,
            string container,
            string? blobName,
            TimeSpan? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                return null;

            var duration = expiry ?? TimeSpan.FromMinutes(30);

            return blobStorage
                .GetReadSasUri(container, blobName, duration)
                .ToString();
        }
    }