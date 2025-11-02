using Catalog.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace Catalog.Infrastructure.Caching;
    public sealed class FusionCacheService : ICacheService
    {
        private readonly IFusionCache _cache;
    private readonly ILogger<FusionCacheService> logger;

    public FusionCacheService(IFusionCache cache, ILogger<FusionCacheService> logger)
        {
            _cache = cache;
        this.logger = logger;
    }

        public async Task<T?> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T?>> factory,
            TimeSpan ttl,
            CancellationToken ct)
        {
       logger.LogInformation($"Cache HIT for {key}");
        return await _cache.GetOrSetAsync<T?>(
                key,
                async _ => await factory(ct),
                options => options.SetDuration(ttl),
                token: ct
            );
        }

        public async Task RemoveAsync(string key, CancellationToken ct)
        {
            await _cache.RemoveAsync(key, token: ct);
        }
    }