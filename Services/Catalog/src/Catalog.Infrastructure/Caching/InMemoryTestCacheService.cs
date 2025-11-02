using Catalog.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace Catalog.Infrastructure.Caching;

public sealed class InMemoryTestCacheService : ICacheService
{
    private readonly IFusionCache _cache;

    public InMemoryTestCacheService()
    {
        // Pure in-memory FusionCache.
        // No Redis, no backplane. Perfect for tests.
        _cache = new FusionCache(
            new FusionCacheOptions(),
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<FusionCache>.Instance
        );
    }

    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T?>> factory,
        TimeSpan ttl,
        CancellationToken ct)
    {
        return await _cache.GetOrSetAsync<T?>(
            key,
            async _ => await factory(ct),
            opt => opt.SetDuration(ttl),
            token: ct
        );
    }

    public Task RemoveAsync(string key, CancellationToken ct)
    {
        // FusionCache returns ValueTask -> convert to Task
        return _cache.RemoveAsync(key, token: ct).AsTask();
    }
}