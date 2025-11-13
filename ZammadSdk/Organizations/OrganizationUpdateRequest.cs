using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Organizations;

/// <summary>
/// Request to update an existing organization.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/organization.html">Organization API</see>
/// </remarks>
public sealed class OrganizationUpdateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("shared")]
    public bool? Shared { get; set; }

    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    [JsonPropertyName("domain_assignment")]
    public bool? DomainAssignment { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("vip")]
    public bool? Vip { get; set; }

    [JsonPropertyName("members")]
    public List<string>? Members { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
