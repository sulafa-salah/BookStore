using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Common.Interfaces;
    public interface IImageJobQueue
    {
        Task EnqueueAsync(Guid bookId, string blobName, string contentType, CancellationToken ct);
    }