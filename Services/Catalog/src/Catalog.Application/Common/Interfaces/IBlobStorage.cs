using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Common.Interfaces;
    public interface IBlobStorage
    {
        Task<string> UploadAsync(Stream content, string contentType, string container, string blobName, CancellationToken ct);
        Uri GetReadSasUri(string container, string blobName, TimeSpan ttl);

       Task DeleteAsync(string container, string blobName, CancellationToken ct);
}