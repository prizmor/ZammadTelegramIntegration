using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.UserAccessTokens;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Lists all access tokens for the current user along with available permissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Response containing tokens and available permissions.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user-access-token.html">User Access Token API</see>
    /// </remarks>
    public async Task<UserAccessTokenListResponse> GetUserAccessTokensAsync(CancellationToken cancellationToken = default)
    {
        return await SendAsync<UserAccessTokenListResponse>(
            HttpMethod.Get,
            "/api/v1/user_access_token",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new access token for the current user.
    /// </summary>
    /// <param name="request">The token creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created access token with the token value (only returned once).</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user-access-token.html">User Access Token API</see>
    /// </remarks>
    public async Task<UserAccessToken> CreateUserAccessTokenAsync(
        UserAccessTokenCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return await SendAsync<UserAccessToken>(
            HttpMethod.Post,
            "/api/v1/user_access_token",
            request,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes an access token by ID.
    /// </summary>
    /// <param name="id">The token ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/user-access-token.html">User Access Token API</see>
    /// </remarks>
    public async Task DeleteUserAccessTokenAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Token ID must be greater than 0.");

        await SendAsync<object>(
            HttpMethod.Delete,
            $"/api/v1/user_access_token/{id}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
