using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Organizations;

/// <summary>
/// Represents a Zammad organization.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/organization.html">Organization API</see>
/// </remarks>
public sealed class Organization
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("shared")]
    public bool Shared { get; set; }

    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    [JsonPropertyName("domain_assignment")]
    public bool DomainAssignment { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("vip")]
    public bool Vip { get; set; }

    [JsonPropertyName("member_ids")]
    public List<long>? MemberIds { get; set; }

    [JsonPropertyName("secondary_member_ids")]
    public List<long>? SecondaryMemberIds { get; set; }

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
