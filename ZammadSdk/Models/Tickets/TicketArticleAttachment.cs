using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tickets;

/// <summary>
/// Represents metadata for a ticket article attachment.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/articles.html">Ticket Articles API</see>
/// </remarks>
public sealed class TicketArticleAttachment
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    [JsonPropertyName("size")]
    public string? Size { get; set; }

    [JsonPropertyName("preferences")]
    public Dictionary<string, JsonElement>? Preferences { get; set; }
}
