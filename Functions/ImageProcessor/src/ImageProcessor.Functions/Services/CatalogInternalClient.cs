using ImageProcessor.Functions.Contracts;
using ImageProcessor.Functions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Functions.Services;
    public sealed class CatalogInternalClient : ICatalogInternalClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<CatalogInternalClient> _logger;
        private readonly ProcessorOptions _opts;

        public CatalogInternalClient(
            IHttpClientFactory factory,
            IOptions<ProcessorOptions> options,
            ILogger<CatalogInternalClient> logger)
        {
            _http = factory.CreateClient(nameof(CatalogInternalClient));
            _opts = options.Value;
            _logger = logger;

            if (!string.IsNullOrWhiteSpace(_opts.InternalAPIKEY))
                _http.DefaultRequestHeaders.Add("X-Internal-Key", _opts.InternalAPIKEY);
        }

        public async Task<bool> UpdateThumbnailAsync(string bookId, string thumbBlobName, CancellationToken ct = default)
        {
            var url = $"{_opts.CatalogBaseURL}/internal/books/{bookId}/thumbnail";
            var req = new UpdateBookThumbnailRequest(thumbBlobName);

            using var resp = await _http.PutAsJsonAsync(url, req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                _logger.LogError("Catalog update failed: {Status} {Body}", resp.StatusCode, body);
                return false; 
            }
            return true;
        }
    }
