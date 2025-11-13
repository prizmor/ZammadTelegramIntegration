using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tags;

/// <summary>
/// Request to remove a tag from an object.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
/// </remarks>
public sealed class TagRemoveRequest
{
    [JsonPropertyName("item")]
    public string? Item { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; } = "Ticket";

    [JsonPropertyName("o_id")]
    public long ObjectId { get; set; }
}
