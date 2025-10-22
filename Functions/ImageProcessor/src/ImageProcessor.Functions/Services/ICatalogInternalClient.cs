using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Functions.Services;
    public interface ICatalogInternalClient
    {
        Task<bool> UpdateThumbnailAsync(string bookId, string thumbBlobName, CancellationToken ct = default);
    }