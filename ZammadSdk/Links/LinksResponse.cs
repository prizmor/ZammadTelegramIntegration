using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Links;

/// <summary>
/// Response containing links for a specific object.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/links.html">Ticket Links API</see>
/// </remarks>
public sealed class LinksResponse
{
    [JsonPropertyName("links")]
    public List<Link>? Links { get; set; }

    [JsonPropertyName("assets")]
    public Dictionary<string, JsonElement>? Assets { get; set; }
}
