using System;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.RealTime.Events;

namespace Zammad.Sdk.RealTime;

/// <summary>
/// Defines real-time ticket monitoring capabilities.
/// </summary>
public interface ITicketMonitor : IAsyncDisposable
{
    event AsyncEventHandler<TicketCreatedEventArgs>? OnTicketCreated;
    event AsyncEventHandler<TicketUpdatedEventArgs>? OnTicketUpdated;
    event AsyncEventHandler<ArticleCreatedEventArgs>? OnArticleCreated;
    event AsyncEventHandler<TicketClosedEventArgs>? OnTicketClosed;

    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);

    ValueTask PublishAsync(TicketCreatedEventArgs args, CancellationToken cancellationToken = default);
    ValueTask PublishAsync(TicketUpdatedEventArgs args, CancellationToken cancellationToken = default);
    ValueTask PublishAsync(ArticleCreatedEventArgs args, CancellationToken cancellationToken = default);
    ValueTask PublishAsync(TicketClosedEventArgs args, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an asynchronous event handler.
/// </summary>
/// <typeparam name="TEventArgs">Type of the event args.</typeparam>
/// <param name="sender">The event sender.</param>
/// <param name="args">The event data.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs args, CancellationToken cancellationToken = default) where TEventArgs : EventArgs;
