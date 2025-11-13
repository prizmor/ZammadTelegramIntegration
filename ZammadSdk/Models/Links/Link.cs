using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Links;

/// <summary>
/// Represents a link between tickets.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/links.html">Ticket Links API</see>
/// </remarks>
public sealed class Link
{
    [JsonPropertyName("link_type")]
    public string? LinkType { get; set; }

    [JsonPropertyName("link_object")]
    public string? LinkObject { get; set; }

    [JsonPropertyName("link_object_value")]
    public long LinkObjectValue { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
