using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tags;

/// <summary>
/// Request to create a new tag in the system.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
/// </remarks>
public sealed class TagCreateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
