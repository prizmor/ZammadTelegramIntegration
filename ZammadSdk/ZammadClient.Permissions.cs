using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Permissions;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Lists all available permissions in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of permissions.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/intro.html">API Introduction</see>
    /// </remarks>
    public async Task<IReadOnlyList<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await SendAsync<List<Permission>>(HttpMethod.Get, "/api/v1/permissions", cancellationToken: cancellationToken).ConfigureAwait(false);
        return permissions;
    }
}
