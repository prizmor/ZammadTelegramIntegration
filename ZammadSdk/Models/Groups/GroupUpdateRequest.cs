using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Groups;

/// <summary>
/// Request to update an existing group.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/group.html">Group API</see>
/// </remarks>
public sealed class GroupUpdateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("signature_id")]
    public long? SignatureId { get; set; }

    [JsonPropertyName("email_address_id")]
    public long? EmailAddressId { get; set; }

    [JsonPropertyName("assignment_timeout")]
    public int? AssignmentTimeout { get; set; }

    [JsonPropertyName("follow_up_possible")]
    public string? FollowUpPossible { get; set; }

    [JsonPropertyName("follow_up_assignment")]
    public bool? FollowUpAssignment { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
