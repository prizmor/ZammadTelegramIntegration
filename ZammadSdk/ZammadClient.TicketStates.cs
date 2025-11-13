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
    /// Lists all ticket states.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of ticket states.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/states.html">Ticket States API</see>
    /// </remarks>
    public async Task<IReadOnlyList<TicketState>> GetTicketStatesAsync(CancellationToken cancellationToken = default)
    {
        var states = await SendAsync<List<TicketState>>(
            HttpMethod.Get,
            "/api/v1/ticket_states",
            cancellationToken: cancellationToken).ConfigureAwait(false);
        return states;
    }

    /// <summary>
    /// Gets a specific ticket state by ID.
    /// </summary>
    /// <param name="id">The state ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ticket state.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/states.html">Ticket States API</see>
    /// </remarks>
    public async Task<TicketState> GetTicketStateAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "State ID must be greater than 0.");

        return await SendAsync<TicketState>(
            HttpMethod.Get,
            $"/api/v1/ticket_states/{id}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new ticket state.
    /// </summary>
    /// <param name="request">The state creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created ticket state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/states.html">Ticket States API</see>
    /// </remarks>
    public async Task<TicketState> CreateTicketStateAsync(
        TicketStateCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<TicketState>(
            HttpMethod.Post,
            "/api/v1/ticket_states",
            request,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing ticket state.
    /// </summary>
    /// <param name="id">The state ID.</param>
    /// <param name="request">The state update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated ticket state.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/states.html">Ticket States API</see>
    /// </remarks>
    public async Task<TicketState> UpdateTicketStateAsync(
        long id,
        TicketStateUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "State ID must be greater than 0.");
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<TicketState>(
            HttpMethod.Put,
            $"/api/v1/ticket_states/{id}",
            request,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a ticket state by ID.
    /// </summary>
    /// <param name="id">The state ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/states.html">Ticket States API</see>
    /// </remarks>
    public async Task DeleteTicketStateAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "State ID must be greater than 0.");

        await SendAsync<object>(
            HttpMethod.Delete,
            $"/api/v1/ticket_states/{id}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
