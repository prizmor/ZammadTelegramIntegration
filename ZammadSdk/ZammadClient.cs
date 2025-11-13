using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Zammad.Sdk.Caching;
using Zammad.Sdk.Core;
using Zammad.Sdk.RealTime;
using Zammad.Sdk.RealTime.Events;

namespace Zammad.Sdk;

/// <summary>
/// Main client for interacting with the Zammad REST API.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/intro.html">API Introduction</see>
/// </remarks>
public sealed partial class ZammadClient : IZammadClient, IDisposable, IAsyncDisposable
{
    private static readonly ActivitySource ActivitySource = new("Zammad.Sdk");
    private static readonly Meter Meter = new("Zammad.Sdk", "2.0.0");
    private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("zammad_sdk_requests_total");
    private static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>("zammad_sdk_request_duration_ms");

    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly RateLimiter _rateLimiter;
    private readonly ICacheProvider? _cache;
    private readonly TimeSpan _cacheDuration;
    private readonly ILogger? _logger;
    private readonly IAsyncPolicy<HttpResponseMessage>? _policy;
    private readonly ITicketMonitor _monitor;
    private readonly bool _ownsMonitor;

    private readonly ITicketsClient _ticketsClient;
    private readonly IUsersClient _usersClient;

    /// <inheritdoc />
    public ITicketsClient Tickets => _ticketsClient;

    /// <inheritdoc />
    public IUsersClient Users => _usersClient;

    /// <summary>
    /// Gets the ticket monitor used for real-time notifications.
    /// </summary>
    public ITicketMonitor Monitor => _monitor;

    /// <summary>
    /// Occurs when a ticket is created.
    /// </summary>
    public event AsyncEventHandler<TicketCreatedEventArgs>? OnTicketCreated
    {
        add => _monitor.OnTicketCreated += value;
        remove => _monitor.OnTicketCreated -= value;
    }

    /// <summary>
    /// Occurs when a ticket is updated.
    /// </summary>
    public event AsyncEventHandler<TicketUpdatedEventArgs>? OnTicketUpdated
    {
        add => _monitor.OnTicketUpdated += value;
        remove => _monitor.OnTicketUpdated -= value;
    }

    /// <summary>
    /// Occurs when a ticket article is created.
    /// </summary>
    public event AsyncEventHandler<ArticleCreatedEventArgs>? OnArticleCreated
    {
        add => _monitor.OnArticleCreated += value;
        remove => _monitor.OnArticleCreated -= value;
    }

    /// <summary>
    /// Occurs when a ticket is closed.
    /// </summary>
    public event AsyncEventHandler<TicketClosedEventArgs>? OnTicketClosed
    {
        add => _monitor.OnTicketClosed += value;
        remove => _monitor.OnTicketClosed -= value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZammadClient"/> class using token authentication.
    /// </summary>
    public ZammadClient(Uri baseUri, string token)
        : this(new ZammadClientOptions { BaseUri = baseUri, Token = token })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZammadClient"/> class using basic authentication.
    /// </summary>
    public ZammadClient(Uri baseUri, string username, string password)
        : this(new ZammadClientOptions { BaseUri = baseUri, Username = username, Password = password })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZammadClient"/> class using a custom HttpClient.
    /// </summary>
    public ZammadClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _disposeHttpClient = false;
        _jsonOptions = CreateJsonOptions();
        _rateLimiter = new RateLimiter(new RateLimitingOptions());
        _cache = null;
        _cacheDuration = TimeSpan.Zero;
        _logger = null;
        _policy = null;
        _monitor = new ZammadTicketMonitor();
        _ownsMonitor = true;
        _ticketsClient = new Resources.TicketsClient(this);
        _usersClient = new Resources.UsersClient(this);
    }

    internal ZammadClient(ZammadClientOptions options, IHttpClientFactory? httpClientFactory, ILogger<ZammadClient>? logger, ITicketMonitor? monitor = null)
    {
        if (options.BaseUri is null)
        {
            throw new ArgumentException("BaseUri must be provided", nameof(options));
        }

        _logger = logger;
        _httpClient = HttpClientManager.CreateHttpClient(options, httpClientFactory, logger);
        _disposeHttpClient = httpClientFactory is null;
        _jsonOptions = options.JsonSerializerOptions ?? CreateJsonOptions();
        _rateLimiter = new RateLimiter(options.RateLimiting);
        _cache = options.Cache.Enabled ? options.Cache.Provider ?? new MemoryCacheProvider() : null;
        _cacheDuration = options.Cache.Duration;
        _policy = BuildPolicy(options);
        _monitor = monitor ?? new ZammadTicketMonitor();
        _ownsMonitor = monitor is null;

        if (!options.Cache.Enabled && options.Cache.Provider != null)
        {
            _logger?.LogWarning("Cache provider provided but caching disabled. Set Cache.Enabled to true to use caching.");
        }

        _ticketsClient = new Resources.TicketsClient(this);
        _usersClient = new Resources.UsersClient(this);
    }

    private static JsonSerializerOptions CreateJsonOptions()
        => new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = null
        };

    private static IAsyncPolicy<HttpResponseMessage>? BuildPolicy(ZammadClientOptions options)
    {
        if (options.RetryPolicy is null && options.CircuitBreakerPolicy is null)
        {
            return null;
        }

        return options.RetryPolicy is null
            ? options.CircuitBreakerPolicy
            : options.CircuitBreakerPolicy is null
                ? options.RetryPolicy
                : Policy.WrapAsync(options.RetryPolicy, options.CircuitBreakerPolicy);
    }

    internal static string BuildUrl(string basePath, Dictionary<string, string?>? queryParams = null)
    {
        if (queryParams == null || queryParams.Count == 0)
        {
            return basePath;
        }

        var query = string.Join("&", queryParams
            .Where(kvp => kvp.Value != null)
            .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value!)}"));

        return string.IsNullOrEmpty(query) ? basePath : $"{basePath}?{query}";
    }

    internal static void AddPaginationParams(Dictionary<string, string?> queryParams, int page, int perPage)
    {
        queryParams["page"] = page.ToString();
        queryParams["per_page"] = perPage.ToString();
    }

    internal static void AddExpandParam(Dictionary<string, string?> queryParams, bool? expand)
    {
        if (expand.HasValue)
        {
            queryParams["expand"] = expand.Value.ToString().ToLowerInvariant();
        }
    }

    internal static void AddLimitParam(Dictionary<string, string?> queryParams, int? limit)
    {
        if (limit.HasValue)
        {
            queryParams["limit"] = limit.Value.ToString();
        }
    }

    internal async Task<T> SendAsync<T>(
        HttpMethod method,
        string relativeUrl,
        object? body = null,
        string? onBehalfOf = null,
        IDictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellationToken = default)
    {
        if (method == HttpMethod.Get && _cache != null)
        {
            var cacheKey = $"{method}:{relativeUrl}:{onBehalfOf}:{JsonSerializer.Serialize(additionalHeaders)}";
            var cached = await _cache.GetAsync<T>(cacheKey, cancellationToken).ConfigureAwait(false);
            if (cached is not null)
            {
                _logger?.LogDebug("Cache hit for {RelativeUrl}", relativeUrl);
                return cached;
            }

            var response = await SendCoreAsync<T>(method, relativeUrl, body, onBehalfOf, additionalHeaders, cancellationToken).ConfigureAwait(false);
            if (_cacheDuration > TimeSpan.Zero)
            {
                await _cache.SetAsync(cacheKey, response, _cacheDuration, cancellationToken).ConfigureAwait(false);
            }
            return response;
        }

        return await SendCoreAsync<T>(method, relativeUrl, body, onBehalfOf, additionalHeaders, cancellationToken).ConfigureAwait(false);
    }

    internal async Task<byte[]> DownloadBinaryAsync(string relativeUrl, CancellationToken cancellationToken)
    {
        return await SendCoreAsync<byte[]>(HttpMethod.Get, relativeUrl, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private async Task<T> SendCoreAsync<T>(
        HttpMethod method,
        string relativeUrl,
        object? body = null,
        string? onBehalfOf = null,
        IDictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity($"ZammadClient.{method.Method}", ActivityKind.Client);
        activity?.SetTag("zammad.url", relativeUrl);
        var request = new HttpRequestMessage(method, relativeUrl);

        if (onBehalfOf != null)
        {
            request.Headers.Add("X-On-Behalf-Of", onBehalfOf);
        }

        if (additionalHeaders != null)
        {
            foreach (var header in additionalHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        if (body != null)
        {
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        await _rateLimiter.ThrottleAsync(cancellationToken).ConfigureAwait(false);

        var start = Stopwatch.GetTimestamp();
        HttpResponseMessage response;
        if (_policy != null)
        {
            response = await _policy.ExecuteAsync((ct) => _httpClient.SendAsync(request, ct), cancellationToken).ConfigureAwait(false);
        }
        else
        {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        var elapsed = Stopwatch.GetElapsedTime(start);
        RequestCounter.Add(1);
        RequestDuration.Record(elapsed.TotalMilliseconds);
        activity?.SetTag("http.status_code", (int)response.StatusCode);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            _rateLimiter.Register429();
        }
        else
        {
            _rateLimiter.ResetBackoff();
        }

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var fullUrl = _httpClient.BaseAddress + relativeUrl;
            _logger?.LogError("Zammad API call failed with status {Status}: {Body}", response.StatusCode, responseBody);
            throw new ZammadApiException(response.StatusCode, fullUrl, responseBody);
        }

        if (typeof(T) == typeof(byte[]))
        {
            var data = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            return (T)(object)data;
        }

        if (typeof(T) == typeof(string))
        {
            var text = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return (T)(object)text;
        }

        if (response.Content.Headers.ContentLength == 0)
        {
            return default!;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, cancellationToken).ConfigureAwait(false);
        return result!;
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Change tracking is managed per resource client; currently no pending state.
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }

        if (_cache is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        Dispose();
        if (_ownsMonitor)
        {
            return _monitor.DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }
}
