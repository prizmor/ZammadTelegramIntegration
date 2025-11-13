using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zammad.Sdk.Core;
using Zammad.Sdk.Tickets;
using Zammad.Sdk.Users;
using Zammad.Sdk.RealTime.Events;

namespace Zammad.Sdk.RealTime;

/// <summary>
/// Background service performing smart polling to simulate real-time updates.
/// </summary>
public sealed class ZammadPollingService : IHostedService, IDisposable
{
    private readonly IZammadClient _client;
    private readonly ITicketMonitor _monitor;
    private readonly ILogger<ZammadPollingService>? _logger;
    private readonly TimeSpan _interval;
    private readonly ConcurrentDictionary<long, Ticket> _knownTickets = new();
    private readonly ConcurrentDictionary<long, User> _userCache = new();
    private CancellationTokenSource? _cts;
    private Task? _pollingTask;

    public ZammadPollingService(
        IZammadClient client,
        ITicketMonitor monitor,
        TimeSpan? interval = null,
        ILogger<ZammadPollingService>? logger = null)
    {
        _client = client;
        _monitor = monitor;
        _logger = logger;
        _interval = interval ?? TimeSpan.FromSeconds(30);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Starting Zammad polling service with interval {Interval}.", _interval);
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _pollingTask = Task.Run(() => PollAsync(_cts.Token), CancellationToken.None);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts is null)
        {
            return;
        }

        _logger?.LogInformation("Stopping Zammad polling service.");
        _cts.Cancel();
        if (_pollingTask != null)
        {
            await Task.WhenAny(_pollingTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
        }
    }

    private async Task PollAsync(CancellationToken cancellationToken)
    {
        await _monitor.StartAsync(cancellationToken).ConfigureAwait(false);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await PollOnceAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while polling Zammad API.");
            }

            await Task.Delay(_interval, cancellationToken).ConfigureAwait(false);
        }

        await _monitor.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task PollOnceAsync(CancellationToken cancellationToken)
    {
        var tickets = await _client.Tickets.GetTicketsAsync(perPage: 200, cancellationToken: cancellationToken).ConfigureAwait(false);
        foreach (var ticket in tickets)
        {
            var snapshot = CloneTicket(ticket);
            if (!_knownTickets.TryGetValue(ticket.Id, out var previous))
            {
                _knownTickets[ticket.Id] = snapshot;
                var firstArticle = await GetFirstArticleAsync(ticket.Id, cancellationToken).ConfigureAwait(false);
                var creator = await GetUserAsync(ticket.CreatedById, cancellationToken).ConfigureAwait(false);
                var createdArgs = new TicketCreatedEventArgs(ticket, firstArticle ?? new TicketArticle { TicketId = ticket.Id }, creator, DateTime.UtcNow);
                await _monitor.PublishAsync(createdArgs, cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (previous.UpdatedAt != ticket.UpdatedAt)
            {
                _knownTickets[ticket.Id] = snapshot;
                var changes = DetectChanges(previous, ticket);
                var updatedBy = await GetUserAsync(ticket.UpdatedById, cancellationToken).ConfigureAwait(false);
                var updatedArgs = new TicketUpdatedEventArgs(ticket, previous, changes, updatedBy);
                await _monitor.PublishAsync(updatedArgs, cancellationToken).ConfigureAwait(false);

                if (ticket.CloseAt.HasValue)
                {
                    var closedBy = await GetUserAsync(ticket.UpdatedById, cancellationToken).ConfigureAwait(false);
                    await _monitor.PublishAsync(new TicketClosedEventArgs(ticket, closedBy, ticket.CloseAt.Value.UtcDateTime), cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    private static IReadOnlyDictionary<string, object?> DetectChanges(Ticket previous, Ticket current)
    {
        var changes = new Dictionary<string, object?>();
        if (previous.StateId != current.StateId)
        {
            changes["state_id"] = current.StateId;
        }
        if (!string.Equals(previous.Title, current.Title, StringComparison.Ordinal))
        {
            changes["title"] = current.Title;
        }
        if (previous.OwnerId != current.OwnerId)
        {
            changes["owner_id"] = current.OwnerId;
        }
        if (previous.PriorityId != current.PriorityId)
        {
            changes["priority_id"] = current.PriorityId;
        }

        return changes;
    }

    private static Ticket CloneTicket(Ticket source)
    {
        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<Ticket>(json)!;
    }

    private async Task<TicketArticle?> GetFirstArticleAsync(long ticketId, CancellationToken cancellationToken)
    {
        var articles = await _client.Tickets.GetTicketArticlesAsync(ticketId, cancellationToken).ConfigureAwait(false);
        return articles.FirstOrDefault();
    }

    private async Task<User> GetUserAsync(long userId, CancellationToken cancellationToken)
    {
        if (_userCache.TryGetValue(userId, out var cached))
        {
            return cached;
        }

        var user = await _client.Users.GetUserAsync(userId, cancellationToken: cancellationToken).ConfigureAwait(false);
        _userCache[userId] = user;
        return user;
    }

    public void Dispose()
    {
        _cts?.Dispose();
    }
}
