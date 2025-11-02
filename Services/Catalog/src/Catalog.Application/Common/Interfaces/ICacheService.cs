using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Common.Interfaces;
    public interface ICacheService
    {
        Task<T?> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T?>> factory,
            TimeSpan ttl,
            CancellationToken ct);

        Task RemoveAsync(string key, CancellationToken ct);
    }