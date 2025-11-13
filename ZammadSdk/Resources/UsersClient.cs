using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Core;
using Zammad.Sdk.Users;

namespace Zammad.Sdk.Resources;

internal sealed class UsersClient : IUsersClient
{
    private readonly ZammadClient _client;

    public UsersClient(ZammadClient client)
    {
        _client = client;
    }

    public Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        => _client.SendAsync<User>(HttpMethod.Get, "/api/v1/users/me", cancellationToken: cancellationToken);

    public async Task<IReadOnlyList<User>> GetUsersAsync(int page = 1, int perPage = 50, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));
        if (perPage <= 0) throw new ArgumentOutOfRangeException(nameof(perPage));

        var query = new Dictionary<string, string?>();
        ZammadClient.AddPaginationParams(query, page, perPage);
        ZammadClient.AddExpandParam(query, expand);
        var url = ZammadClient.BuildUrl("/api/v1/users", query);
        return await _client.SendAsync<List<User>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<User> GetUserAsync(long id, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

        var query = new Dictionary<string, string?>();
        ZammadClient.AddExpandParam(query, expand);
        var url = ZammadClient.BuildUrl($"/api/v1/users/{id}", query);
        return await _client.SendAsync<User>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public Task<User> CreateUserAsync(UserCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<User>(HttpMethod.Post, "/api/v1/users", request, cancellationToken: cancellationToken);
    }

    public Task<User> UpdateUserAsync(long id, UserUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (request == null) throw new ArgumentNullException(nameof(request));
        return _client.SendAsync<User>(HttpMethod.Put, $"/api/v1/users/{id}", request, cancellationToken: cancellationToken);
    }

    public Task DeleteUserAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        return _client.SendAsync<object>(HttpMethod.Delete, $"/api/v1/users/{id}", cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<User>> SearchUsersAsync(string query, int? limit = null, bool? expand = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("Query cannot be null or whitespace.", nameof(query));
        var parameters = new Dictionary<string, string?>
        {
            ["query"] = query
        };
        ZammadClient.AddLimitParam(parameters, limit);
        ZammadClient.AddExpandParam(parameters, expand);
        var url = ZammadClient.BuildUrl("/api/v1/users/search", parameters);
        return await _client.SendAsync<List<User>>(HttpMethod.Get, url, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
