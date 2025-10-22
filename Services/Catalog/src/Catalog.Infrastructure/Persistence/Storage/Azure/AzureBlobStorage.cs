using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Catalog.Application.Common.Interfaces;

namespace Catalog.Infrastructure.Persistence.Storage.Azure;
public  class AzureBlobStorage(BlobServiceClient client) : IBlobStorage
{
    public async Task<string> UploadAsync(
        Stream content,
        string contentType,
        string container,
        string blobName,
        CancellationToken ct = default)
    {
        var cont = client.GetBlobContainerClient(container);
        await cont.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

        var blob = cont.GetBlobClient(blobName);

        await blob.UploadAsync(
            content,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            },
            ct);

        return blob.Name;
    }

    public async Task DeleteAsync(string container, string blobName, CancellationToken ct = default)
    {
        var cont = client.GetBlobContainerClient(container);
        await cont.GetBlobClient(blobName)
                  .DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: ct);
    }

    public Uri GetReadSasUri(string container, string blobName, TimeSpan ttl)
    {
        var blob = client.GetBlobContainerClient(container).GetBlobClient(blobName);

        if (blob.CanGenerateSasUri)
        {
            var sas = new BlobSasBuilder
            {
                BlobContainerName = container,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
            };
            sas.SetPermissions(BlobSasPermissions.Read);
            return blob.GenerateSasUri(sas);
        }

        // Local/Azurite fallback (no SAS capability)
        return blob.Uri;
    }
}