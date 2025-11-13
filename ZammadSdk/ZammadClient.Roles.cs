using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Roles;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Lists all roles with pagination support.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 50).</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of roles.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/role.html">Role API</see> and
    /// <see href="https://docs.zammad.org/en/latest/api/intro.html#pagination">Pagination</see>
    /// </remarks>
    public async Task<IReadOnlyList<Role>> GetRolesAsync(
        int page = 1,
        int perPage = 50,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
        if (perPage <= 0) throw new ArgumentOutOfRangeException(nameof(perPage), "PerPage must be greater than 0.");

        var url = $"/api/v1/roles?page={page}&per_page={perPage}";
        if (expand.HasValue)
        {
            url += $"&expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        var roles = await SendAsync<List<Role>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
        return roles;
    }

    /// <summary>
    /// Gets a specific role by ID.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The role.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/role.html">Role API</see>
    /// </remarks>
    public async Task<Role> GetRoleAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Role ID must be greater than 0.");

        var url = $"/api/v1/roles/{id}";
        if (expand.HasValue)
        {
            url += $"?expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        return await SendAsync<Role>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="request">The role creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created role.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/role.html">Role API</see>
    /// </remarks>
    public async Task<Role> CreateRoleAsync(RoleCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<Role>(HttpMethod.Post, "/api/v1/roles", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="request">The role update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated role.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/role.html">Role API</see>
    /// </remarks>
    public async Task<Role> UpdateRoleAsync(long id, RoleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Role ID must be greater than 0.");
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<Role>(HttpMethod.Put, $"/api/v1/roles/{id}", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a role by ID.
    /// </summary>
    /// <param name="id">The role ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/role.html">Role API</see>
    /// </remarks>
    public async Task DeleteRoleAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Role ID must be greater than 0.");

        await SendAsync<object>(HttpMethod.Delete, $"/api/v1/roles/{id}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for roles matching the query.
    /// </summary>
    /// <param name="query">Search query string.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching roles.</returns>
    /// <exception cref="ArgumentException">Thrown when query is null or whitespace.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/role.html">Role API</see>
    /// </remarks>
    public async Task<IReadOnlyList<Role>> SearchRolesAsync(
        string query,
        int? limit = null,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("Query cannot be null or whitespace.", nameof(query));

        var url = $"/api/v1/roles/search?query={Uri.EscapeDataString(query)}";
        if (limit.HasValue)
        {
            url += $"&limit={limit.Value}";
        }
        if (expand.HasValue)
        {
            url += $"&expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        var roles = await SendAsync<List<Role>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
        return roles;
    }
}
