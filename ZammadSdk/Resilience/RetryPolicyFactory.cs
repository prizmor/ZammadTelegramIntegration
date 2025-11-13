using System;
using System.Net;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace Zammad.Sdk.Resilience;

/// <summary>
/// Provides helpers for creating resilience policies.
/// </summary>
public static class RetryPolicyFactory
{
    /// <summary>
    /// Creates a retry policy suitable for the Zammad API.
    /// </summary>
    /// <param name="maxRetries">Maximum retry count.</param>
    /// <returns>The retry policy.</returns>
    public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(int maxRetries)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(message => message.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(maxRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
