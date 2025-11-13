using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zammad.Sdk.RealTime.Events;

namespace Zammad.Sdk.RealTime;

/// <summary>
/// Default implementation of <see cref="ITicketMonitor"/> that acts as an event hub for real-time updates.
/// </summary>
public sealed class ZammadTicketMonitor : ITicketMonitor
{
    private readonly ILogger? _logger;
    private bool _started;

    public ZammadTicketMonitor(ILogger<ZammadTicketMonitor>? logger = null)
    {
        _logger = logger;
    }

    public event AsyncEventHandler<TicketCreatedEventArgs>? OnTicketCreated;
    public event AsyncEventHandler<TicketUpdatedEventArgs>? OnTicketUpdated;
    public event AsyncEventHandler<ArticleCreatedEventArgs>? OnArticleCreated;
    public event AsyncEventHandler<TicketClosedEventArgs>? OnTicketClosed;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _started = true;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _started = false;
        return Task.CompletedTask;
    }

    public ValueTask PublishAsync(TicketCreatedEventArgs args, CancellationToken cancellationToken = default)
        => InvokeAsync(OnTicketCreated, args, cancellationToken);

    public ValueTask PublishAsync(TicketUpdatedEventArgs args, CancellationToken cancellationToken = default)
        => InvokeAsync(OnTicketUpdated, args, cancellationToken);

    public ValueTask PublishAsync(ArticleCreatedEventArgs args, CancellationToken cancellationToken = default)
        => InvokeAsync(OnArticleCreated, args, cancellationToken);

    public ValueTask PublishAsync(TicketClosedEventArgs args, CancellationToken cancellationToken = default)
        => InvokeAsync(OnTicketClosed, args, cancellationToken);

    public ValueTask DisposeAsync()
    {
        _started = false;
        return ValueTask.CompletedTask;
    }

    private async ValueTask InvokeAsync<T>(AsyncEventHandler<T>? handler, T args, CancellationToken cancellationToken)
        where T : EventArgs
    {
        if (!_started)
        {
            _logger?.LogDebug("Ignoring event {EventType} because monitor is not started.", typeof(T).Name);
            return;
        }

        if (handler is null)
        {
            return;
        }

        foreach (AsyncEventHandler<T> invocation in handler.GetInvocationList())
        {
            try
            {
                await invocation(this, args, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error executing event handler for {EventType}", typeof(T).Name);
            }
        }
    }
}
