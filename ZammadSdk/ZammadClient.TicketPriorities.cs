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
    /// Lists all ticket priorities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of ticket priorities.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/priorities.html">Ticket Priorities API</see>
    /// </remarks>
    public async Task<IReadOnlyList<TicketPriority>> GetTicketPrioritiesAsync(CancellationToken cancellationToken = default)
    {
        var priorities = await SendAsync<List<TicketPriority>>(
            HttpMethod.Get,
            "/api/v1/ticket_priorities",
            cancellationToken: cancellationToken).ConfigureAwait(false);
        return priorities;
    }

    /// <summary>
    /// Gets a specific ticket priority by ID.
    /// </summary>
    /// <param name="id">The priority ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ticket priority.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/priorities.html">Ticket Priorities API</see>
    /// </remarks>
    public async Task<TicketPriority> GetTicketPriorityAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Priority ID must be greater than 0.");

        return await SendAsync<TicketPriority>(
            HttpMethod.Get,
            $"/api/v1/ticket_priorities/{id}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new ticket priority.
    /// </summary>
    /// <param name="request">The priority creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created ticket priority.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/priorities.html">Ticket Priorities API</see>
    /// </remarks>
    public async Task<TicketPriority> CreateTicketPriorityAsync(
        TicketPriorityCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<TicketPriority>(
            HttpMethod.Post,
            "/api/v1/ticket_priorities",
            request,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing ticket priority.
    /// </summary>
    /// <param name="id">The priority ID.</param>
    /// <param name="request">The priority update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated ticket priority.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/priorities.html">Ticket Priorities API</see>
    /// </remarks>
    public async Task<TicketPriority> UpdateTicketPriorityAsync(
        long id,
        TicketPriorityUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Priority ID must be greater than 0.");
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<TicketPriority>(
            HttpMethod.Put,
            $"/api/v1/ticket_priorities/{id}",
            request,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a ticket priority by ID.
    /// </summary>
    /// <param name="id">The priority ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/priorities.html">Ticket Priorities API</see>
    /// </remarks>
    public async Task DeleteTicketPriorityAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Priority ID must be greater than 0.");

        await SendAsync<object>(
            HttpMethod.Delete,
            $"/api/v1/ticket_priorities/{id}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
