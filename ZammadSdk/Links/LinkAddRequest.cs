using System.Text.Json.Serialization;

namespace Zammad.Sdk.Links;

/// <summary>
/// Request to create a link between tickets.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/links.html">Ticket Links API</see>
/// </remarks>
public sealed class LinkAddRequest
{
    [JsonPropertyName("link_type")]
    public string? LinkType { get; set; }

    [JsonPropertyName("link_object_source")]
    public string? LinkObjectSource { get; set; }

    [JsonPropertyName("link_object_source_number")]
    public string? LinkObjectSourceNumber { get; set; }

    [JsonPropertyName("link_object_target")]
    public string? LinkObjectTarget { get; set; }

    [JsonPropertyName("link_object_target_value")]
    public long LinkObjectTargetValue { get; set; }
}
