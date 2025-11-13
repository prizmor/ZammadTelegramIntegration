using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Zammad.Sdk.Caching;

/// <summary>
/// <see cref="ICacheProvider"/> implementation backed by <see cref="MemoryCache"/>.
/// </summary>
public sealed class MemoryCacheProvider : ICacheProvider, IDisposable
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var value) && value is T typed)
        {
            return ValueTask.FromResult<T?>(typed);
        }

        return ValueTask.FromResult<T?>(default);
    }

    public ValueTask SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        _cache.Set(key, value!, ttl);
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _cache.Dispose();
    }
}
