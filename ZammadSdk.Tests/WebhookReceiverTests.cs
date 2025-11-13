using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Zammad.Sdk.Core;
using Zammad.Sdk.RealTime;
using Zammad.Sdk.RealTime.Events;
using Zammad.Sdk.Tickets;
using Zammad.Sdk.Users;

namespace Zammad.Sdk.Tests;

public sealed class WebhookReceiverTests
{
    private const string Secret = "test-secret";

    [Fact]
    public async Task HandleAsync_InvalidSignature_Returns401()
    {
        var monitor = new FakeMonitor();
        var client = new StubClient();
        var receiver = new ZammadWebhookReceiver(client, monitor, new ZammadWebhookOptions { Secret = Secret }, NullLogger<ZammadWebhookReceiver>.Instance);
        var context = CreateContext("ticket.created", "{}", signature: "invalid");

        await receiver.HandleAsync(context, CancellationToken.None);

        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        Assert.Empty(monitor.CreatedEvents);
    }

    [Fact]
    public async Task HandleAsync_ValidSignature_RaisesEvent()
    {
        var monitor = new FakeMonitor();
        var client = new StubClient();
        var payload = JsonSerializer.Serialize(new
        {
            ticket = new Ticket { Id = 1, CreatedById = 7, UpdatedById = 7 },
            article = new TicketArticle { TicketId = 1 }
        });
        var context = CreateContext("ticket.created", payload, secret: Secret);
        var receiver = new ZammadWebhookReceiver(client, monitor, new ZammadWebhookOptions { Secret = Secret }, NullLogger<ZammadWebhookReceiver>.Instance);

        await receiver.HandleAsync(context, CancellationToken.None);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
        Assert.Single(monitor.CreatedEvents);
        Assert.Equal(1, monitor.CreatedEvents[0].Ticket.Id);
    }

    private static DefaultHttpContext CreateContext(string eventName, string payload, string? signature = null, string? secret = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/webhooks/zammad";
        context.Request.Method = "POST";
        context.Request.ContentType = "application/json";
        context.Request.Headers["X-Zammad-Event"] = eventName;
        if (secret != null)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            signature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
        }
        if (signature != null)
        {
            context.Request.Headers["X-Zammad-Signature"] = signature;
        }
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(payload));
        return context;
    }

    private sealed class FakeMonitor : ITicketMonitor
    {
        public List<TicketCreatedEventArgs> CreatedEvents { get; } = new();

        public event AsyncEventHandler<TicketCreatedEventArgs>? OnTicketCreated;
        public event AsyncEventHandler<TicketUpdatedEventArgs>? OnTicketUpdated;
        public event AsyncEventHandler<ArticleCreatedEventArgs>? OnArticleCreated;
        public event AsyncEventHandler<TicketClosedEventArgs>? OnTicketClosed;

        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask PublishAsync(TicketCreatedEventArgs args, CancellationToken cancellationToken = default)
        {
            CreatedEvents.Add(args);
            return ValueTask.CompletedTask;
        }

        public ValueTask PublishAsync(TicketUpdatedEventArgs args, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
        public ValueTask PublishAsync(ArticleCreatedEventArgs args, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
        public ValueTask PublishAsync(TicketClosedEventArgs args, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
    }

    private sealed class StubClient : IZammadClient
    {
        public StubClient()
        {
            Users = new StubUsersClient();
            Tickets = new StubTicketsClient();
        }

        public ITicketsClient Tickets { get; }
        public IUsersClient Users { get; }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        private sealed class StubUsersClient : IUsersClient
        {
            public Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default) => Task.FromResult(new User());

            public Task<IReadOnlyList<User>> GetUsersAsync(int page = 1, int perPage = 50, CancellationToken cancellationToken = default)
                => Task.FromResult<IReadOnlyList<User>>(Array.Empty<User>());

            public Task<User> GetUserAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
                => Task.FromResult(new User { Id = id });

            public Task<User> CreateUserAsync(UserCreateRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult(new User());

            public Task<User> UpdateUserAsync(long id, UserUpdateRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult(new User());

            public Task DeleteUserAsync(long id, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task<IReadOnlyList<User>> SearchUsersAsync(string query, int? limit = null, bool? expand = null, CancellationToken cancellationToken = default)
                => Task.FromResult<IReadOnlyList<User>>(Array.Empty<User>());
        }

        private sealed class StubTicketsClient : ITicketsClient
        {
            public Task<Ticket> CreateTicketAsync(TicketCreateRequest request, string? onBehalfOf = null, CancellationToken cancellationToken = default)
                => Task.FromResult(new Ticket());

            public Task DeleteTicketAsync(long id, CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task<Ticket> GetTicketAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
                => Task.FromResult(new Ticket { Id = id });

            public Task<IReadOnlyList<Ticket>> GetTicketsAsync(int page = 1, int perPage = 50, bool? expand = null, CancellationToken cancellationToken = default)
                => Task.FromResult<IReadOnlyList<Ticket>>(Array.Empty<Ticket>());

            public Task<IReadOnlyList<TicketArticle>> GetTicketArticlesAsync(long ticketId, CancellationToken cancellationToken = default)
                => Task.FromResult<IReadOnlyList<TicketArticle>>(Array.Empty<TicketArticle>());

            public Task<Ticket> UpdateTicketAsync(long id, TicketUpdateRequest request, string? onBehalfOf = null, CancellationToken cancellationToken = default)
                => Task.FromResult(new Ticket { Id = id });
        }
    }
}
