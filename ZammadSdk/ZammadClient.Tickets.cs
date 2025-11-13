using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Tickets;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Lists all tickets with pagination support.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 50).</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of tickets.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/index.html">Ticket API</see> and
    /// <see href="https://docs.zammad.org/en/latest/api/intro.html#pagination">Pagination</see>
    /// </remarks>
    public async Task<IReadOnlyList<Ticket>> GetTicketsAsync(
        int page = 1,
        int perPage = 50,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
        if (perPage <= 0) throw new ArgumentOutOfRangeException(nameof(perPage), "PerPage must be greater than 0.");

        var url = $"/api/v1/tickets?page={page}&per_page={perPage}";
        if (expand.HasValue)
        {
            url += $"&expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        var tickets = await SendAsync<List<Ticket>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
        return tickets;
    }

    /// <summary>
    /// Gets a specific ticket by ID.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ticket.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/index.html">Ticket API</see>
    /// </remarks>
    public async Task<Ticket> GetTicketAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Ticket ID must be greater than 0.");

        var url = $"/api/v1/tickets/{id}";
        if (expand.HasValue)
        {
            url += $"?expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        return await SendAsync<Ticket>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new ticket.
    /// </summary>
    /// <param name="request">The ticket creation request.</param>
    /// <param name="onBehalfOf">Optional user ID to create the ticket on behalf of.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created ticket.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when title or article is missing.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/index.html">Ticket API</see> and
    /// <see href="https://docs.zammad.org/en/latest/api/intro.html#actions-on-behalf-of-other-users">On Behalf Of</see>
    /// </remarks>
    public async Task<Ticket> CreateTicketAsync(
        TicketCreateRequest request,
        string? onBehalfOf = null,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Title)) throw new ArgumentException("Title is required.", nameof(request));
        if (request.Article == null) throw new ArgumentException("Article is required.", nameof(request));

        return await SendAsync<Ticket>(HttpMethod.Post, "/api/v1/tickets", request, onBehalfOf, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing ticket.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="request">The ticket update request.</param>
    /// <param name="onBehalfOf">Optional user ID to update the ticket on behalf of.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated ticket.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/index.html">Ticket API</see> and
    /// <see href="https://docs.zammad.org/en/latest/api/intro.html#actions-on-behalf-of-other-users">On Behalf Of</see>
    /// </remarks>
    public async Task<Ticket> UpdateTicketAsync(
        long id,
        TicketUpdateRequest request,
        string? onBehalfOf = null,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Ticket ID must be greater than 0.");
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<Ticket>(HttpMethod.Put, $"/api/v1/tickets/{id}", request, onBehalfOf, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a ticket by ID.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/index.html">Ticket API</see>
    /// </remarks>
    public async Task DeleteTicketAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Ticket ID must be greater than 0.");

        await SendAsync<object>(HttpMethod.Delete, $"/api/v1/tickets/{id}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for tickets matching the query.
    /// </summary>
    /// <param name="query">Search query string.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching tickets.</returns>
    /// <exception cref="ArgumentException">Thrown when query is null or whitespace.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/index.html">Ticket API</see>
    /// </remarks>
    public async Task<IReadOnlyList<Ticket>> SearchTicketsAsync(
        string query,
        int? limit = null,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("Query cannot be null or whitespace.", nameof(query));

        var url = $"/api/v1/tickets/search?query={Uri.EscapeDataString(query)}";
        if (limit.HasValue)
        {
            url += $"&limit={limit.Value}";
        }
        if (expand.HasValue)
        {
            url += $"&expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        var tickets = await SendAsync<List<Ticket>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
        return tickets;
    }
}
