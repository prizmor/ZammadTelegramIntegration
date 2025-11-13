using System;
using System.Net;

namespace Zammad.Sdk;

/// <summary>
/// Exception thrown when Zammad API returns an error status code.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/intro.html">API Introduction</see>
/// </remarks>
public sealed class ZammadApiException : Exception
{
    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the URL that was requested.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Gets the raw response body from the API.
    /// </summary>
    public string ResponseBody { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZammadApiException"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="url">The requested URL.</param>
    /// <param name="responseBody">The response body.</param>
    public ZammadApiException(HttpStatusCode statusCode, string url, string responseBody)
        : base($"Zammad API request failed with status {(int)statusCode} {statusCode} for {url}: {responseBody}")
    {
        StatusCode = statusCode;
        Url = url;
        ResponseBody = responseBody;
    }
}
