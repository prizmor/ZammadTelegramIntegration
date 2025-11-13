using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Zammad.Sdk.RealTime;

namespace Zammad.Sdk.Extensions;

/// <summary>
/// Provides ASP.NET Core extensions for enabling Zammad webhooks.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the Zammad webhook middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="configure">Optional monitor configuration callback.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseZammadWebhooks(this IApplicationBuilder app, Action<ITicketMonitor>? configure = null)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (configure != null)
        {
            var monitor = app.ApplicationServices.GetRequiredService<ITicketMonitor>();
            configure(monitor);
        }

        var options = app.ApplicationServices.GetRequiredService<ZammadWebhookOptions>();
        return app.UseMiddleware<ZammadWebhookMiddleware>(options);
    }
}
