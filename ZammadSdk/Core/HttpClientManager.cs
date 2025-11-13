using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace Zammad.Sdk.Core;

/// <summary>
/// Provides helpers for creating <see cref="HttpClient"/> instances.
/// </summary>
internal static class HttpClientManager
{
    public static HttpClient CreateHttpClient(ZammadClientOptions options, IHttpClientFactory? factory, ILogger? logger)
    {
        if (options.BaseUri is null)
        {
            throw new InvalidOperationException("ZammadClientOptions.BaseUri must be provided.");
        }

        HttpClient client;
        if (factory != null)
        {
            client = factory.CreateClient("Zammad.Sdk");
            if (client.BaseAddress == null)
            {
                client.BaseAddress = options.BaseUri;
            }
        }
        else
        {
            client = new HttpClient
            {
                BaseAddress = options.BaseUri,
                Timeout = options.Timeout
            };
        }

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        ConfigureAuthentication(client, options, logger);
        return client;
    }

    private static void ConfigureAuthentication(HttpClient client, ZammadClientOptions options, ILogger? logger)
    {
        if (!string.IsNullOrWhiteSpace(options.Token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", $"token={options.Token}");
            return;
        }

        if (!string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password))
        {
            var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{options.Username}:{options.Password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            return;
        }

        logger?.LogWarning("Zammad client created without authentication headers. Ensure the API allows anonymous access.");
    }
}
