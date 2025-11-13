using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.UserAccessTokens;

/// <summary>
/// Request to create a new user access token.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/user-access-token.html">User Access Token API</see>
/// </remarks>
public sealed class UserAccessTokenCreateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("permissions")]
    public List<string>? Permissions { get; set; }

    [JsonPropertyName("expires_at")]
    public string? ExpiresAt { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
