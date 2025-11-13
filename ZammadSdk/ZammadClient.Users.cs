using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Users;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Gets the current authenticated user information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current user.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user.html">User API</see>
    /// </remarks>
    public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        return await Users.GetCurrentUserAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all users with pagination support.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 50).</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user.html">User API</see> and
    /// <see href="https://docs.zammad.org/en/latest/api/intro.html#pagination">Pagination</see>
    /// </remarks>
    public async Task<IReadOnlyList<User>> GetUsersAsync(
        int page = 1,
        int perPage = 50,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        return await Users.GetUsersAsync(page, perPage, expand, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a specific user by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user.html">User API</see>
    /// </remarks>
    public async Task<User> GetUserAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
    {
        return await Users.GetUserAsync(id, expand, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The user creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user.html">User API</see>
    /// </remarks>
    public async Task<User> CreateUserAsync(UserCreateRequest request, CancellationToken cancellationToken = default)
    {
        return await Users.CreateUserAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="request">The user update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user.html">User API</see>
    /// </remarks>
    public async Task<User> UpdateUserAsync(long id, UserUpdateRequest request, CancellationToken cancellationToken = default)
    {
        return await Users.UpdateUserAsync(id, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a user by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user.html">User API</see>
    /// </remarks>
    public async Task DeleteUserAsync(long id, CancellationToken cancellationToken = default)
    {
        await Users.DeleteUserAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for users matching the query.
    /// </summary>
    /// <param name="query">Search query string.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching users.</returns>
    /// <exception cref="ArgumentException">Thrown when query is null or whitespace.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user.html">User API</see>
    /// </remarks>
    public async Task<IReadOnlyList<User>> SearchUsersAsync(
        string query,
        int? limit = null,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        return await Users.SearchUsersAsync(query, limit, expand, cancellationToken).ConfigureAwait(false);
    }
}
