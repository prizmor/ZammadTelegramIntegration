using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Zammad.Sdk;

/// <summary>
/// Main client for interacting with the Zammad REST API.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/intro.html">API Introduction</see>
/// </remarks>
public sealed partial class ZammadClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZammadClient"/> class using token authentication.
    /// </summary>
    /// <param name="baseUri">The base URI of the Zammad instance.</param>
    /// <param name="token">The personal access token.</param>
    /// <exception cref="ArgumentNullException">Thrown when baseUri or token is null.</exception>
    /// <exception cref="ArgumentException">Thrown when token is empty or whitespace.</exception>
    public ZammadClient(Uri baseUri, string token)
    {
        if (baseUri == null) throw new ArgumentNullException(nameof(baseUri));
        if (token == null) throw new ArgumentNullException(nameof(token));
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token cannot be empty or whitespace.", nameof(token));

        _httpClient = new HttpClient { BaseAddress = baseUri };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", $"token={token}");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _disposeHttpClient = true;
        _jsonOptions = CreateJsonOptions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZammadClient"/> class using basic authentication.
    /// </summary>
    /// <param name="baseUri">The base URI of the Zammad instance.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when username or password is empty or whitespace.</exception>
    public ZammadClient(Uri baseUri, string username, string password)
    {
        if (baseUri == null) throw new ArgumentNullException(nameof(baseUri));
        if (username == null) throw new ArgumentNullException(nameof(username));
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be empty or whitespace.", nameof(username));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

        _httpClient = new HttpClient { BaseAddress = baseUri };
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _disposeHttpClient = true;
        _jsonOptions = CreateJsonOptions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZammadClient"/> class using a custom HttpClient.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use. Must have BaseAddress set and appropriate authentication headers.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient is null.</exception>
    public ZammadClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _disposeHttpClient = false;
        _jsonOptions = CreateJsonOptions();
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        return new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = null // Explicit [JsonPropertyName] attributes are used on all DTOs
        };
    }

    private static string BuildUrl(string basePath, Dictionary<string, string?>? queryParams = null)
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

    private static void AddPaginationParams(Dictionary<string, string?> queryParams, int page, int perPage)
    {
        queryParams["page"] = page.ToString();
        queryParams["per_page"] = perPage.ToString();
    }

    private static void AddExpandParam(Dictionary<string, string?> queryParams, bool? expand)
    {
        if (expand.HasValue)
        {
            queryParams["expand"] = expand.Value.ToString().ToLowerInvariant();
        }
    }

    private static void AddLimitParam(Dictionary<string, string?> queryParams, int? limit)
    {
        if (limit.HasValue)
        {
            queryParams["limit"] = limit.Value.ToString();
        }
    }

    private async Task<T> SendAsync<T>(
        HttpMethod method,
        string relativeUrl,
        object? body = null,
        string? onBehalfOf = null,
        IDictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, relativeUrl);

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

        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var fullUrl = _httpClient.BaseAddress + relativeUrl;
            throw new ZammadApiException(response.StatusCode, fullUrl, responseBody);
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)responseBody;
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return default!;
        }

        return JsonSerializer.Deserialize<T>(responseBody, _jsonOptions)!;
    }

    private async Task<byte[]> DownloadBinaryAsync(
        string relativeUrl,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var fullUrl = _httpClient.BaseAddress + relativeUrl;
            throw new ZammadApiException(response.StatusCode, fullUrl, responseBody);
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Disposes resources used by the client.
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient?.Dispose();
        }
    }
}
