using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tags;

/// <summary>
/// Represents a tag in the system.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
/// </remarks>
public sealed class Tag
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}
