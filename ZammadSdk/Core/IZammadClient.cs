using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Tickets;
using Zammad.Sdk.Users;

namespace Zammad.Sdk.Core;

/// <summary>
/// Represents the main entry point for interacting with the Zammad API.
/// </summary>
public interface IZammadClient
{
    /// <summary>
    /// Gets the tickets resource client.
    /// </summary>
    ITicketsClient Tickets { get; }

    /// <summary>
    /// Gets the users resource client.
    /// </summary>
    IUsersClient Users { get; }

    /// <summary>
    /// Persists pending changes tracked by the client.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Abstraction for tickets resource operations.
/// </summary>
public interface ITicketsClient
{
    Task<IReadOnlyList<Ticket>> GetTicketsAsync(int page = 1, int perPage = 50, bool? expand = null, CancellationToken cancellationToken = default);
    Task<Ticket> GetTicketAsync(long id, bool? expand = null, CancellationToken cancellationToken = default);
    Task<Ticket> CreateTicketAsync(TicketCreateRequest request, string? onBehalfOf = null, CancellationToken cancellationToken = default);
    Task<Ticket> UpdateTicketAsync(long id, TicketUpdateRequest request, string? onBehalfOf = null, CancellationToken cancellationToken = default);
    Task DeleteTicketAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketArticle>> GetTicketArticlesAsync(long ticketId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Abstraction for users resource operations.
/// </summary>
public interface IUsersClient
{
    Task<IReadOnlyList<User>> GetUsersAsync(int page = 1, int perPage = 50, CancellationToken cancellationToken = default);
    Task<User> GetUserAsync(long id, bool? expand = null, CancellationToken cancellationToken = default);
    Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(UserCreateRequest request, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(long id, UserUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> SearchUsersAsync(string query, int? limit = null, bool? expand = null, CancellationToken cancellationToken = default);
}
