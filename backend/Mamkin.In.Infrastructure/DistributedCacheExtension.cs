using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Mamkin.In.Infrastructure;

public static class DistributedCacheExtension
{
    public static async Task<T?> GetValue<T>(
        this IDistributedCache cache, 
        string key,
        CancellationToken cancellationToken)  where T : class
    {
        var jsonData = await cache.GetStringAsync(key, cancellationToken);
        if (jsonData == null)
            return null;

        var result = JsonSerializer.Deserialize<T>(jsonData)!;
        return result;
    }
    
    public static async Task SetValue<T>(this IDistributedCache cache, 
        string key, 
        T value, 
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken)  where T : class
    {
        var data = JsonSerializer.Serialize(value);
        
        await cache.SetStringAsync(key, data, options, cancellationToken);
    }
}