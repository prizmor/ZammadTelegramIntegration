using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tickets;

/// <summary>
/// Request to create a new ticket priority.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/priorities.html">Ticket Priorities API</see>
/// </remarks>
public sealed class TicketPriorityCreateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("default_create")]
    public bool? DefaultCreate { get; set; }

    [JsonPropertyName("ui_icon")]
    public string? UiIcon { get; set; }

    [JsonPropertyName("ui_color")]
    public string? UiColor { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
