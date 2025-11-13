using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zammad.Sdk.Core;
using Zammad.Sdk.RealTime;
using Zammad.Sdk.RealTime.Events;

namespace ZammadExample;

internal sealed class SampleWorker : BackgroundService
{
    private readonly IZammadClient _client;
    private readonly ITicketMonitor _monitor;
    private readonly ILogger<SampleWorker> _logger;

    public SampleWorker(IZammadClient client, ITicketMonitor monitor, ILogger<SampleWorker> logger)
    {
        _client = client;
        _monitor = monitor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.OnTicketCreated += HandleTicketCreatedAsync;
        _client.OnTicketUpdated += HandleTicketUpdatedAsync;
        await _monitor.StartAsync(stoppingToken).ConfigureAwait(false);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _monitor.StopAsync(cancellationToken).ConfigureAwait(false);
        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    private Task HandleTicketCreatedAsync(object sender, TicketCreatedEventArgs args, CancellationToken token)
    {
        _logger.LogInformation("Ticket {TicketId} created at {Timestamp}.", args.Ticket.Id, args.Timestamp);
        return Task.CompletedTask;
    }

    private Task HandleTicketUpdatedAsync(object sender, TicketUpdatedEventArgs args, CancellationToken token)
    {
        _logger.LogInformation("Ticket {TicketId} updated. Changed fields: {Fields}.", args.Ticket.Id, string.Join(",", args.Changes.Keys));
        return Task.CompletedTask;
    }
}
