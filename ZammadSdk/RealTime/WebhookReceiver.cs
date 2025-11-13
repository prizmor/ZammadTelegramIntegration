using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Zammad.Sdk.Core;
using Zammad.Sdk.RealTime.Events;
using Zammad.Sdk.Tickets;
using Zammad.Sdk.Users;

namespace Zammad.Sdk.RealTime;

/// <summary>
/// Handles webhook notifications from Zammad.
/// </summary>
public sealed class ZammadWebhookReceiver
{
    private readonly IZammadClient _client;
    private readonly ITicketMonitor _monitor;
    private readonly ZammadWebhookOptions _options;
    private readonly ILogger<ZammadWebhookReceiver>? _logger;

    public ZammadWebhookReceiver(
        IZammadClient client,
        ITicketMonitor monitor,
        ZammadWebhookOptions options,
        ILogger<ZammadWebhookReceiver>? logger = null)
    {
        _client = client;
        _monitor = monitor;
        _options = options;
        _logger = logger;
    }

    public async Task HandleAsync(HttpContext context, CancellationToken cancellationToken)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync().ConfigureAwait(false);
        context.Request.Body.Position = 0;

        if (!ValidateSignature(body, context.Request.Headers[_options.SignatureHeaderName]))
        {
            _logger?.LogWarning("Invalid webhook signature received.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var eventName = context.Request.Headers[_options.EventHeaderName];
        if (string.IsNullOrWhiteSpace(eventName))
        {
            _logger?.LogWarning("Missing webhook event header {Header}.", _options.EventHeaderName);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await ProcessEventAsync(eventName!, body, cancellationToken).ConfigureAwait(false);
        context.Response.StatusCode = StatusCodes.Status200OK;
    }

    private bool ValidateSignature(string payload, string signatureHeader)
    {
        if (string.IsNullOrWhiteSpace(_options.Secret))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            return false;
        }

        if (signatureHeader.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase))
        {
            signatureHeader = signatureHeader[7..];
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.Secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = Convert.ToHexString(hash).ToLowerInvariant();
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(computedSignature), Encoding.UTF8.GetBytes(signatureHeader));
    }

    private async Task ProcessEventAsync(string eventName, string body, CancellationToken cancellationToken)
    {
        using var document = JsonDocument.Parse(body);
        if (!document.RootElement.TryGetProperty("ticket", out var ticketElement))
        {
            _logger?.LogDebug("Webhook payload missing ticket element: {Body}", body);
            return;
        }

        var ticket = JsonSerializer.Deserialize<Ticket>(ticketElement.GetRawText());
        if (ticket is null)
        {
            _logger?.LogWarning("Unable to deserialize ticket from webhook payload: {Body}", body);
            return;
        }

        switch (eventName)
        {
            case "ticket.created":
                await HandleTicketCreatedAsync(document.RootElement, ticket, cancellationToken).ConfigureAwait(false);
                break;
            case "ticket.updated":
                await HandleTicketUpdatedAsync(document.RootElement, ticket, cancellationToken).ConfigureAwait(false);
                break;
            case "ticket.article.created":
                await HandleArticleCreatedAsync(document.RootElement, ticket, cancellationToken).ConfigureAwait(false);
                break;
            case "ticket.closed":
                await HandleTicketClosedAsync(document.RootElement, ticket, cancellationToken).ConfigureAwait(false);
                break;
            default:
                _logger?.LogInformation("Unhandled webhook event {Event}", eventName);
                break;
        }
    }

    private async Task HandleTicketCreatedAsync(JsonElement root, Ticket ticket, CancellationToken cancellationToken)
    {
        var article = DeserializeArticle(root);
        var creator = await GetUserAsync(ticket.CreatedById, cancellationToken).ConfigureAwait(false);
        var args = new TicketCreatedEventArgs(ticket, article ?? new TicketArticle { TicketId = ticket.Id }, creator, DateTime.UtcNow);
        await _monitor.PublishAsync(args, cancellationToken).ConfigureAwait(false);
    }

    private async Task HandleTicketUpdatedAsync(JsonElement root, Ticket ticket, CancellationToken cancellationToken)
    {
        var previous = root.TryGetProperty("previous", out var previousElement)
            ? JsonSerializer.Deserialize<Ticket>(previousElement.GetRawText()) ?? ticket
            : ticket;
        var changes = root.TryGetProperty("changes", out var changesElement)
            ? JsonSerializer.Deserialize<Dictionary<string, object?>>(changesElement.GetRawText()) ?? new Dictionary<string, object?>()
            : new Dictionary<string, object?>();
        var updatedBy = await GetUserAsync(ticket.UpdatedById, cancellationToken).ConfigureAwait(false);
        var args = new TicketUpdatedEventArgs(ticket, previous, changes, updatedBy);
        await _monitor.PublishAsync(args, cancellationToken).ConfigureAwait(false);
    }

    private async Task HandleArticleCreatedAsync(JsonElement root, Ticket ticket, CancellationToken cancellationToken)
    {
        var article = DeserializeArticle(root) ?? new TicketArticle { TicketId = ticket.Id };
        var isSplit = root.TryGetProperty("is_split", out var split) && split.GetBoolean();
        long? fromTicketId = null;
        long? fromArticleId = null;
        if (root.TryGetProperty("split_from_ticket_id", out var splitTicket))
        {
            fromTicketId = splitTicket.GetInt64();
        }
        if (root.TryGetProperty("split_from_article_id", out var splitArticle))
        {
            fromArticleId = splitArticle.GetInt64();
        }

        var args = new ArticleCreatedEventArgs(article, ticket, isSplit, fromTicketId, fromArticleId);
        await _monitor.PublishAsync(args, cancellationToken).ConfigureAwait(false);
    }

    private async Task HandleTicketClosedAsync(JsonElement root, Ticket ticket, CancellationToken cancellationToken)
    {
        var closedBy = await GetUserAsync(ticket.UpdatedById, cancellationToken).ConfigureAwait(false);
        var closedAt = root.TryGetProperty("closed_at", out var closedAtElement)
            ? closedAtElement.GetDateTime()
            : DateTime.UtcNow;
        var args = new TicketClosedEventArgs(ticket, closedBy, closedAt);
        await _monitor.PublishAsync(args, cancellationToken).ConfigureAwait(false);
    }

    private static TicketArticle? DeserializeArticle(JsonElement root)
    {
        if (root.TryGetProperty("article", out var articleElement))
        {
            return JsonSerializer.Deserialize<TicketArticle>(articleElement.GetRawText());
        }

        return null;
    }

    private async Task<User> GetUserAsync(long userId, CancellationToken cancellationToken)
    {
        return await _client.Users.GetUserAsync(userId, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Options for webhook processing.
/// </summary>
public sealed record ZammadWebhookOptions
{
    public string Path { get; init; } = "/webhooks/zammad";
    public string? Secret { get; init; }
    public string SignatureHeaderName { get; init; } = "X-Zammad-Signature";
    public string EventHeaderName { get; init; } = "X-Zammad-Event";
}
