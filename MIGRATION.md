# Migration Guide

This document highlights the changes required to migrate from the legacy `ZammadClient` implementation to the
new production-ready architecture delivered in this release.

## Summary of Breaking Changes

- The `ZammadClient` class now implements the `IZammadClient` interface and exposes resource-specific clients through
  the `Tickets` and `Users` properties. Existing top-level methods remain available for backward compatibility, but new
  code should prefer the resource clients.
- Dependency injection is now the recommended way to create clients. Constructors are still available for manual usage,
  but the preferred pattern is `services.AddZammadClient(...)`.
- Real-time notifications are provided through the `ITicketMonitor` abstraction. Subscribe via the `ZammadClient`
  events (`OnTicketCreated`, `OnTicketUpdated`, etc.) or directly through the monitor instance.

## Step-by-Step Migration

1. **Update Package References**
   - Reference the new version of `Zammad.Sdk` (requires .NET 6 or newer).
   - Add `Microsoft.Extensions.Http`, `Microsoft.Extensions.Logging`, and `Polly` packages to your host if you
     are not already using them.

2. **Switch to DI Registration**
   - Replace manual `new ZammadClient(...)` calls inside ASP.NET Core or Worker Service projects with:
     ```csharp
     services.AddZammadClient(options =>
     {
         options.BaseUri = new Uri(Configuration["Zammad:Url"]);
         options.Token = Configuration["Zammad:Token"];
     });
     ```
   - If you relied on long-lived static clients, inject `IZammadClient` instead.

3. **Configure Real-time Monitoring**
   - For webhook scenarios, register middleware:
     ```csharp
     services.AddZammadWebhooks(o => o.Secret = Configuration["Zammad:WebhookSecret"]);
     app.UseZammadWebhooks(monitor =>
     {
         monitor.OnTicketCreated += ...;
     });
     ```
   - For polling-based monitoring, register the hosted service:
     ```csharp
     services.AddZammadPolling(TimeSpan.FromSeconds(30));
     ```

4. **Adopt Resilience & Rate Limiting Settings**
   - Use `ZammadClientOptions` to enable retries, circuit breaking, caching, or offline queueing.
     ```csharp
     services.AddZammadClient(options =>
     {
         options.BaseUri = new Uri(...);
         options.Token = ...;
         options.MaxRetries = 5;
         options.Cache = new CacheOptions { Enabled = true, Duration = TimeSpan.FromMinutes(10) };
     });
     ```

5. **Testing Adjustments**
   - Replace direct `ZammadClient` mocks with the new interfaces (`IZammadClient`, `ITicketsClient`, `IUsersClient`).
   - Use the provided fake implementations in the test project as reference when building custom fakes.

## Optional Enhancements

- Enable structured logging by injecting `ILogger<ZammadClient>` when constructing the client manually.
- Provide a custom `ICacheProvider` to plug in distributed caching (Redis, SQL, etc.).
- Implement an offline queue by persisting failed requests through the `EnableOfflineQueue` options.

## Need Help?

Open an issue in the repository with the migration steps you've attempted and your configuration. The maintainers
will assist you with any edge cases.
