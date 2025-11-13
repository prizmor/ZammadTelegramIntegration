using System;
using Microsoft.Extensions.DependencyInjection;
using Zammad.Sdk.Core;
using Zammad.Sdk.RealTime;
using Zammad.Sdk.Resilience;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zammad.Sdk.Extensions;

/// <summary>
/// Provides dependency injection extensions for the Zammad SDK.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Zammad client and supporting services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddZammadClient(this IServiceCollection services, Action<ZammadClientOptionsBuilder> configure)
    {
        var builder = new ZammadClientOptionsBuilder();
        configure(builder);
        var options = builder.Build();
        if (options.EnableRetryPolicy && options.RetryPolicy is null)
        {
            options = options with { RetryPolicy = RetryPolicyFactory.CreateRetryPolicy(options.MaxRetries) };
        }
        if (options.EnableCircuitBreaker && options.CircuitBreakerPolicy is null)
        {
            options = options with { CircuitBreakerPolicy = Resilience.CircuitBreakerPolicy.CreateDefault() };
        }

        services.AddSingleton(options);
        services.AddHttpClient("Zammad.Sdk", client =>
        {
            if (options.BaseUri is null)
            {
                throw new InvalidOperationException("ZammadClientOptions.BaseUri must be provided.");
            }

            client.BaseAddress = options.BaseUri;
            client.Timeout = options.Timeout;
            client.DefaultRequestVersion = new Version(2, 0);
            client.DefaultVersionPolicy = System.Net.Http.HttpVersionPolicy.RequestVersionOrLower;
        });

        services.AddSingleton<ITicketMonitor, ZammadTicketMonitor>();

        services.AddSingleton(provider =>
        {
            var logger = provider.GetService<Microsoft.Extensions.Logging.ILogger<ZammadClient>>();
            var factory = provider.GetService<System.Net.Http.IHttpClientFactory>();
            var monitor = provider.GetRequiredService<ITicketMonitor>();
            return new ZammadClient(options, factory, logger, monitor);
        });
        services.AddSingleton<IZammadClient>(sp => sp.GetRequiredService<ZammadClient>());

        return services;
    }

    /// <summary>
    /// Adds the webhook middleware with the provided configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Webhook configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddZammadWebhooks(this IServiceCollection services, Action<ZammadWebhookOptions>? configure = null)
    {
        var options = new ZammadWebhookOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddSingleton<ZammadWebhookReceiver>();
        return services;
    }

    /// <summary>
    /// Adds the polling background service for environments where webhooks are not available.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="interval">Optional polling interval.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddZammadPolling(this IServiceCollection services, TimeSpan? interval = null)
    {
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IZammadClient>();
            var monitor = sp.GetRequiredService<ITicketMonitor>();
            var logger = sp.GetService<ILogger<ZammadPollingService>>();
            return new ZammadPollingService(client, monitor, interval, logger);
        });
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<ZammadPollingService>());
        return services;
    }
}
