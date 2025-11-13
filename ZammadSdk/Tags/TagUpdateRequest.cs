using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tags;

/// <summary>
/// Request to update (rename) a tag.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
/// </remarks>
public sealed class TagUpdateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
