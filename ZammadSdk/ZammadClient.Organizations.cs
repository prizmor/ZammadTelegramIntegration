using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Organizations;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Lists all organizations with pagination support.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 50).</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of organizations.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/organization.html">Organization API</see> and
    /// <see href="https://docs.zammad.org/en/latest/api/intro.html#pagination">Pagination</see>
    /// </remarks>
    public async Task<IReadOnlyList<Organization>> GetOrganizationsAsync(
        int page = 1,
        int perPage = 50,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0.");
        if (perPage <= 0) throw new ArgumentOutOfRangeException(nameof(perPage), "PerPage must be greater than 0.");

        var url = $"/api/v1/organizations?page={page}&per_page={perPage}";
        if (expand.HasValue)
        {
            url += $"&expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        var organizations = await SendAsync<List<Organization>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
        return organizations;
    }

    /// <summary>
    /// Gets a specific organization by ID.
    /// </summary>
    /// <param name="id">The organization ID.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The organization.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/organization.html">Organization API</see>
    /// </remarks>
    public async Task<Organization> GetOrganizationAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Organization ID must be greater than 0.");

        var url = $"/api/v1/organizations/{id}";
        if (expand.HasValue)
        {
            url += $"?expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        return await SendAsync<Organization>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new organization.
    /// </summary>
    /// <param name="request">The organization creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created organization.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/organization.html">Organization API</see>
    /// </remarks>
    public async Task<Organization> CreateOrganizationAsync(OrganizationCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<Organization>(HttpMethod.Post, "/api/v1/organizations", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing organization.
    /// </summary>
    /// <param name="id">The organization ID.</param>
    /// <param name="request">The organization update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated organization.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/organization.html">Organization API</see>
    /// </remarks>
    public async Task<Organization> UpdateOrganizationAsync(long id, OrganizationUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Organization ID must be greater than 0.");
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<Organization>(HttpMethod.Put, $"/api/v1/organizations/{id}", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes an organization by ID.
    /// </summary>
    /// <param name="id">The organization ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/organization.html">Organization API</see>
    /// </remarks>
    public async Task DeleteOrganizationAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Organization ID must be greater than 0.");

        await SendAsync<object>(HttpMethod.Delete, $"/api/v1/organizations/{id}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Searches for organizations matching the query.
    /// </summary>
    /// <param name="query">Search query string.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <param name="expand">Whether to expand relations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching organizations.</returns>
    /// <exception cref="ArgumentException">Thrown when query is null or whitespace.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/organization.html">Organization API</see>
    /// </remarks>
    public async Task<IReadOnlyList<Organization>> SearchOrganizationsAsync(
        string query,
        int? limit = null,
        bool? expand = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("Query cannot be null or whitespace.", nameof(query));

        var url = $"/api/v1/organizations/search?query={Uri.EscapeDataString(query)}";
        if (limit.HasValue)
        {
            url += $"&limit={limit.Value}";
        }
        if (expand.HasValue)
        {
            url += $"&expand={expand.Value.ToString().ToLowerInvariant()}";
        }

        var organizations = await SendAsync<List<Organization>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
        return organizations;
    }
}
