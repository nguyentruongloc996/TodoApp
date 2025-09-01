using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TodoApp.Infrastructure.Services
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }

    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedBytes = await _cache.GetAsync(key);
            if (cachedBytes == null)
                return default;

            try
            {
                var jsonString = System.Text.Encoding.UTF8.GetString(cachedBytes);
                return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
            }
            catch (JsonException)
            {
                // If deserialization fails, remove the invalid cache entry
                await _cache.RemoveAsync(key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var jsonValue = JsonSerializer.Serialize(value, _jsonOptions);
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonValue);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                // Default expiration of 1 hour
                options.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            }

            await _cache.SetAsync(key, bytes, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            // Note: Redis doesn't support pattern-based deletion directly
            // This is a simplified implementation. In production, you might want to use
            // Redis SCAN command or maintain a list of keys for pattern-based deletion
            throw new NotImplementedException("Pattern-based deletion requires additional Redis implementation");
        }
    }
} 