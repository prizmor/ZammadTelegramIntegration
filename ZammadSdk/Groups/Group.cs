using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Groups;

/// <summary>
/// Represents a Zammad group.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/group.html">Group API</see>
/// </remarks>
public sealed class Group
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

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
    public bool FollowUpAssignment { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("user_ids")]
    public List<long>? UserIds { get; set; }

    [JsonPropertyName("created_by_id")]
    public long CreatedById { get; set; }

    [JsonPropertyName("updated_by_id")]
    public long UpdatedById { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
