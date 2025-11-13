using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Groups;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Lists all groups with pagination support.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 50).</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of groups.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/group.html">Group API</see> and
    /// <see href="https://docs.zammad.org/en/latest/api/intro.html#pagination">Pagination</see>
    /// </remarks>
    public async Task<IReadOnlyList<Group>> GetGroupsAsync(
        int page = 1,
        int perPage = 50,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
        if (perPage <= 0) throw new ArgumentOutOfRangeException(nameof(perPage), "PerPage must be greater than 0.");

        var url = $"/api/v1/groups?page={page}&per_page={perPage}";
        if (expand.HasValue)
        {
            url += $"&expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        var groups = await SendAsync<List<Group>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
        return groups;
    }

    /// <summary>
    /// Gets a specific group by ID.
    /// </summary>
    /// <param name="id">The group ID.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The group.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/group.html">Group API</see>
    /// </remarks>
    public async Task<Group> GetGroupAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Group ID must be greater than 0.");

        var url = $"/api/v1/groups/{id}";
        if (expand.HasValue)
        {
            url += $"?expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        return await SendAsync<Group>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="request">The group creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created group.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/group.html">Group API</see>
    /// </remarks>
    public async Task<Group> CreateGroupAsync(GroupCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<Group>(HttpMethod.Post, "/api/v1/groups", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing group.
    /// </summary>
    /// <param name="id">The group ID.</param>
    /// <param name="request">The group update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated group.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/group.html">Group API</see>
    /// </remarks>
    public async Task<Group> UpdateGroupAsync(long id, GroupUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Group ID must be greater than 0.");
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<Group>(HttpMethod.Put, $"/api/v1/groups/{id}", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a group by ID.
    /// </summary>
    /// <param name="id">The group ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/group.html">Group API</see>
    /// </remarks>
    public async Task DeleteGroupAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Group ID must be greater than 0.");

        await SendAsync<object>(HttpMethod.Delete, $"/api/v1/groups/{id}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for groups matching the query.
    /// </summary>
    /// <param name="query">Search query string.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching groups.</returns>
    /// <exception cref="ArgumentException">Thrown when query is null or whitespace.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/group.html">Group API</see>
    /// </remarks>
    public async Task<IReadOnlyList<Group>> SearchGroupsAsync(
        string query,
        int? limit = null,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("Query cannot be null or whitespace.", nameof(query));

        var url = $"/api/v1/groups/search?query={Uri.EscapeDataString(query)}";
        if (limit.HasValue)
        {
            url += $"&limit={limit.Value}";
        }
        if (expand.HasValue)
        {
            url += $"&expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        var groups = await SendAsync<List<Group>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
        return groups;
    }
}
