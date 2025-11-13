using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;

namespace Zammad.Sdk.Core;

/// <summary>
/// Represents configuration options for <see cref="ZammadClient"/> instances.
/// </summary>
public sealed record ZammadClientOptions
{
    /// <summary>
    /// Gets the base URL of the Zammad instance.
    /// </summary>
    public Uri? BaseUri { get; init; }

    /// <summary>
    /// Gets the personal access token used for authentication.
    /// </summary>
    public string? Token { get; init; }

    /// <summary>
    /// Gets the username for basic authentication.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Gets the password for basic authentication.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Gets a value indicating whether retry policy is enabled.
    /// </summary>
    public bool EnableRetryPolicy { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether circuit breaker policy is enabled.
    /// </summary>
    public bool EnableCircuitBreaker { get; init; } = true;

    /// <summary>
    /// Gets the maximum retry attempts for transient errors.
    /// </summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    /// Gets the timeout applied to outgoing HTTP requests.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(100);

    /// <summary>
    /// Gets the custom retry policy, if any.
    /// </summary>
    public IAsyncPolicy<HttpResponseMessage>? RetryPolicy { get; init; }

    /// <summary>
    /// Gets the custom circuit breaker policy, if any.
    /// </summary>
    public IAsyncPolicy<HttpResponseMessage>? CircuitBreakerPolicy { get; init; }

    /// <summary>
    /// Gets the request rate limiting configuration.
    /// </summary>
    public RateLimitingOptions RateLimiting { get; init; } = new();

    /// <summary>
    /// Gets the optional cache configuration.
    /// </summary>
    public CacheOptions Cache { get; init; } = new();

    /// <summary>
    /// Gets a value indicating whether offline queue is enabled.
    /// </summary>
    public bool EnableOfflineQueue { get; init; }

    /// <summary>
    /// Gets the path to the queue storage when offline queue is enabled.
    /// </summary>
    public string? QueueStoragePath { get; init; }

    /// <summary>
    /// Gets the JSON serializer options to use.
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; init; } = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Gets the optional logger instance.
    /// </summary>
    public ILogger? Logger { get; init; }
}

/// <summary>
/// Builder used to construct immutable <see cref="ZammadClientOptions"/> instances.
/// </summary>
public sealed class ZammadClientOptionsBuilder
{
    private ZammadClientOptions _options = new();

    public Uri? BaseUri { set => _options = _options with { BaseUri = value }; }
    public string? Token { set => _options = _options with { Token = value }; }
    public string? Username { set => _options = _options with { Username = value }; }
    public string? Password { set => _options = _options with { Password = value }; }
    public bool EnableRetryPolicy { set => _options = _options with { EnableRetryPolicy = value }; }
    public bool EnableCircuitBreaker { set => _options = _options with { EnableCircuitBreaker = value }; }
    public int MaxRetries { set => _options = _options with { MaxRetries = value }; }
    public TimeSpan Timeout { set => _options = _options with { Timeout = value }; }
    public IAsyncPolicy<HttpResponseMessage>? RetryPolicy { set => _options = _options with { RetryPolicy = value }; }
    public IAsyncPolicy<HttpResponseMessage>? CircuitBreakerPolicy { set => _options = _options with { CircuitBreakerPolicy = value }; }
    public RateLimitingOptions RateLimiting { set => _options = _options with { RateLimiting = value }; }
    public CacheOptions Cache { set => _options = _options with { Cache = value }; }
    public bool EnableOfflineQueue { set => _options = _options with { EnableOfflineQueue = value }; }
    public string? QueueStoragePath { set => _options = _options with { QueueStoragePath = value }; }
    public JsonSerializerOptions JsonSerializerOptions { set => _options = _options with { JsonSerializerOptions = value }; }
    public ILogger? Logger { set => _options = _options with { Logger = value }; }

    /// <summary>
    /// Builds the immutable options instance.
    /// </summary>
    /// <returns>The options.</returns>
    public ZammadClientOptions Build() => _options;
}

/// <summary>
/// Represents cache configuration options.
/// </summary>
public sealed record CacheOptions
{
    /// <summary>
    /// Gets a value indicating whether caching is enabled.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// Gets the cache provider used for read operations.
    /// </summary>
    public Caching.ICacheProvider? Provider { get; init; }

    /// <summary>
    /// Gets the cache entry lifetime.
    /// </summary>
    public TimeSpan Duration { get; init; } = TimeSpan.FromMinutes(5);
}

/// <summary>
/// Represents rate limiting configuration options.
/// </summary>
public sealed record RateLimitingOptions
{
    /// <summary>
    /// Gets a value indicating whether rate limiting is enabled.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets the maximum number of requests per time window.
    /// </summary>
    public int RequestsPerWindow { get; init; } = 60;

    /// <summary>
    /// Gets the length of the rate limiting window.
    /// </summary>
    public TimeSpan Window { get; init; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets the backoff multiplier applied after receiving <c>429 Too Many Requests</c>.
    /// </summary>
    public double BackoffMultiplier { get; init; } = 2.0d;
}
