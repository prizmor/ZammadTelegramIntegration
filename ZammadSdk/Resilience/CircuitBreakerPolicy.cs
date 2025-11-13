using System;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace Zammad.Sdk.Resilience;

/// <summary>
/// Provides default circuit breaker policy for the SDK.
/// </summary>
public static class CircuitBreakerPolicy
{
    /// <summary>
    /// Creates a default circuit breaker policy.
    /// </summary>
    /// <returns>The circuit breaker policy.</returns>
    public static IAsyncPolicy<HttpResponseMessage> CreateDefault()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}
