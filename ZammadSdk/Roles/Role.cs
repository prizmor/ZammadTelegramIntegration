using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Roles;

/// <summary>
/// Represents a Zammad role.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/role.html">Role API</see>
/// </remarks>
public sealed class Role
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("default_at_signup")]
    public bool DefaultAtSignup { get; set; }

    [JsonPropertyName("preferences")]
    public Dictionary<string, JsonElement>? Preferences { get; set; }

    [JsonPropertyName("permission_ids")]
    public List<long>? PermissionIds { get; set; }

    [JsonPropertyName("knowledge_base_permission_ids")]
    public List<long>? KnowledgeBasePermissionIds { get; set; }

    [JsonPropertyName("group_ids")]
    public Dictionary<string, List<string>>? GroupIds { get; set; }

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
