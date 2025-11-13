# Zammad .NET SDK

Zammad.Sdk is a production-ready .NET client for the [Zammad](https://zammad.org/) helpdesk platform. It offers
strongly typed API access, resilience policies, realtime monitoring via webhooks or polling, and first-class
integration with the Microsoft.Extensions hosting and dependency injection ecosystem.

## Features

- ✅ Coverage for core REST endpoints with typed DTOs and async APIs
- ✅ Integrated retry/circuit-breaker policies using [Polly](https://github.com/App-vNext/Polly)
- ✅ Adaptive rate limiting and optional response caching
- ✅ Real-time ticket monitoring through ASP.NET Core webhooks or smart polling background services
- ✅ Typed event model (`TicketCreatedEventArgs`, `TicketUpdatedEventArgs`, etc.)
- ✅ Fluent dependency injection helpers (`AddZammadClient`, `AddZammadWebhooks`, `AddZammadPolling`)
- ✅ Observability hooks with structured logging, metrics and tracing
- ✅ Extensible architecture with mock-friendly interfaces for unit testing

## Getting Started

Install the SDK from your local build or package feed, then register it with the service container:

```csharp
builder.Services.AddZammadClient(options =>
{
    options.BaseUri = new Uri(configuration["Zammad:Url"]);
    options.Token = configuration["Zammad:Token"];
    options.Cache = new CacheOptions { Enabled = true, Duration = TimeSpan.FromMinutes(5) };
});

builder.Services.AddZammadWebhooks(o =>
{
    o.Path = "/webhooks/zammad";
    o.Secret = configuration["Zammad:WebhookSecret"];
});

builder.Services.AddZammadPolling(TimeSpan.FromSeconds(30));
```

Inject `IZammadClient` wherever you need to work with the API:

```csharp
public sealed class TicketService
{
    private readonly IZammadClient _client;

    public TicketService(IZammadClient client)
    {
        _client = client;
    }

    public async Task CloseTicketAsync(long id, CancellationToken cancellationToken)
    {
        var ticket = await _client.Tickets.GetTicketAsync(id, cancellationToken: cancellationToken);
        ticket.StateId = 4; // closed
        await _client.Tickets.UpdateTicketAsync(id, new TicketUpdateRequest { StateId = 4 }, cancellationToken: cancellationToken);
    }
}
```

## Real-time Monitoring

The SDK exposes a unified monitor that works with both webhooks and polling. Subscribe to events from the client:

```csharp
await monitor.StartAsync(cancellationToken);

client.OnTicketCreated += async (sender, args, token) =>
{
    logger.LogInformation("Ticket {Id} created", args.Ticket.Id);
    await Task.CompletedTask;
};
```

The sample project under `Host/` demonstrates an end-to-end console host that wires up the client, background polling
service, and webhook middleware.

## Testing Support

The `ZammadSdk.Tests` project contains reusable fakes for key interfaces and verifies webhook signature handling.
You can implement your own in-memory clients by targeting the `IZammadClient`, `ITicketsClient`, and `IUsersClient`
interfaces.

## Building & Running

```bash
# restore packages
dotnet restore

# run unit tests
dotnet test

# launch sample host
cd Host
dotnet run
```

> **Note**: The commands above require the .NET 8 SDK.

## License

Distributed under the MIT license. See [LICENSE](LICENSE) for more information.
