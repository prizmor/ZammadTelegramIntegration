using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tickets;

/// <summary>
/// Request to create a new ticket state.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/states.html">Ticket States API</see>
/// </remarks>
public sealed class TicketStateCreateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("state_type_id")]
    public long? StateTypeId { get; set; }

    [JsonPropertyName("next_state_id")]
    public long? NextStateId { get; set; }

    [JsonPropertyName("ignore_escalation")]
    public bool? IgnoreEscalation { get; set; }

    [JsonPropertyName("default_create")]
    public bool? DefaultCreate { get; set; }

    [JsonPropertyName("default_follow_up")]
    public bool? DefaultFollowUp { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
