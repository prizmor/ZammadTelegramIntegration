using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zammad.Sdk.Core;

namespace Zammad.Sdk.RealTime;

/// <summary>
/// ASP.NET Core middleware that receives Zammad webhook notifications.
/// </summary>
public sealed class ZammadWebhookMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ZammadWebhookOptions _options;

    public ZammadWebhookMiddleware(RequestDelegate next, ZammadWebhookOptions options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (string.Equals(context.Request.Path, _options.Path, StringComparison.OrdinalIgnoreCase))
        {
            var receiver = context.RequestServices.GetRequiredService<ZammadWebhookReceiver>();
            var logger = context.RequestServices.GetService<ILogger<ZammadWebhookMiddleware>>();
            try
            {
                await receiver.HandleAsync(context, context.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error while handling Zammad webhook request.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return;
        }

        await _next(context).ConfigureAwait(false);
    }
}
