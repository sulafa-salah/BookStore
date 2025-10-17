using Cart.Application.Common.Interfaces;
using Cart.Domain.Entities;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cart.Infrastructure.Redis;
    public sealed class RedisCartRepository : ICartRepository
    {
    // Redis database connection
    private readonly IDatabase _db;
    // Default time-to-live (TTL) for cache entries
    private readonly TimeSpan _defaultTtl;

    // Namespace prefix for Redis keys (helps isolate environments like dev/staging/prod)
    private readonly string _ns;
    // Predefined JSON options for consistent serialization/deserialization

    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // use camelCase for JSON keys
        WriteIndented = false // minimize payload size
    };

    // Constructor — gets a Redis connection and configuration settings (TTL, Namespace)
    public RedisCartRepository(IConnectionMultiplexer mux, IOptions<RedisOptions> opts)
        {
            _db = mux.GetDatabase(); // get the Redis database instance
        _defaultTtl = TimeSpan.FromMinutes(opts.Value.DefaultTtlMinutes); // default expiry time
        _ns = opts.Value.Namespace ?? "dev"; // fallback to "dev" if no namespace provided
    }

    // Helper to build a Redis key using namespace + "cart" + userId
    private string Key(string userId) => $"{_ns}:cart:{userId}";

    // Retrieve the user's cart from Redis
    public async Task<ShoppingCart?> GetAsync(string userId, CancellationToken ct = default)
        {
            var key = Key(userId);
            var val = await _db.StringGetAsync(key); // get the JSON value from Redis
        if (val.IsNullOrEmpty) return null; // return null if cart doesn't exist

            // Refresh TTL (extend expiration) each time the cart is read
            await _db.KeyExpireAsync(key, _defaultTtl);
        // Convert JSON back into a ShoppingCart object
        return JsonSerializer.Deserialize<ShoppingCart>(val!, _json);
        }
    // Save or update a user's cart in Redis with optional custom TTL
    public async Task SaveAsync(ShoppingCart cart, TimeSpan? ttl = null, CancellationToken ct = default)
        {
        // Convert the cart object to JSON
        var payload = JsonSerializer.Serialize(cart, _json);
        // Store it in Redis with a TTL (default or custom)
        await _db.StringSetAsync(Key(cart.UserId), payload, ttl ?? _defaultTtl);
        }
    // Remove a user's cart from Redis
    public Task<bool> RemoveAsync(string userId, CancellationToken ct = default)
            => _db.KeyDeleteAsync(Key(userId));
    }
