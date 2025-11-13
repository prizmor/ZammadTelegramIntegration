using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Core;
using Zammad.Sdk.Tickets;

namespace Zammad.Sdk.Resources;

internal sealed class TicketsClient : ITicketsClient
{
    private readonly ZammadClient _client;

    public TicketsClient(ZammadClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<Ticket>> GetTicketsAsync(int page = 1, int perPage = 50, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
        if (perPage <= 0) throw new ArgumentOutOfRangeException(nameof(perPage));

        var query = new Dictionary<string, string?>();
        ZammadClient.AddPaginationParams(query, page, perPage);
        ZammadClient.AddExpandParam(query, expand);
        var url = ZammadClient.BuildUrl("/api/v1/tickets", query);
        return await _client.SendAsync<List<Ticket>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<Ticket> GetTicketAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

        var query = new Dictionary<string, string?>();
        ZammadClient.AddExpandParam(query, expand);
        var url = ZammadClient.BuildUrl($"/api/v1/tickets/{id}", query);
        return await _client.SendAsync<Ticket>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<Ticket> CreateTicketAsync(TicketCreateRequest request, string? onBehalfOf = null, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Title)) throw new ArgumentException("Title is required.", nameof(request));
        if (request.Article == null) throw new ArgumentException("Article is required.", nameof(request));

        return await _client.SendAsync<Ticket>(HttpMethod.Post, "/api/v1/tickets", request, onBehalfOf, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<Ticket> UpdateTicketAsync(long id, TicketUpdateRequest request, string? onBehalfOf = null, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await _client.SendAsync<Ticket>(HttpMethod.Put, $"/api/v1/tickets/{id}", request, onBehalfOf, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteTicketAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

        await _client.SendAsync<object>(HttpMethod.Delete, $"/api/v1/tickets/{id}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TicketArticle>> GetTicketArticlesAsync(long ticketId, CancellationToken cancellationToken = default)
    {
        if (ticketId <= 0) throw new ArgumentOutOfRangeException(nameof(ticketId));

        return await _client.SendAsync<List<TicketArticle>>(HttpMethod.Get, $"/api/v1/ticket_articles/by_ticket/{ticketId}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
