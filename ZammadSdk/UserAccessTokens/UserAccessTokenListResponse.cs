using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.UserAccessTokens;

/// <summary>
/// Response containing access tokens and available permissions.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/user-access-token.html">User Access Token API</see>
/// </remarks>
public sealed class UserAccessTokenListResponse
{
    [JsonPropertyName("tokens")]
    public List<UserAccessToken>? Tokens { get; set; }

    [JsonPropertyName("permissions")]
    public List<string>? Permissions { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
