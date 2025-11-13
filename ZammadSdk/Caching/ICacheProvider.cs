using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zammad.Sdk.Caching;

/// <summary>
/// Represents a cache provider abstraction.
/// </summary>
public interface ICacheProvider
{
    ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    ValueTask SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default);
    ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);
}
