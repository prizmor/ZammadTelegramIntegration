using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Users;

/// <summary>
/// Request to update an existing user.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/user.html">User API</see>
/// </remarks>
public sealed class UserUpdateRequest
{
    [JsonPropertyName("login")]
    public string? Login { get; set; }

    [JsonPropertyName("firstname")]
    public string? Firstname { get; set; }

    [JsonPropertyName("lastname")]
    public string? Lastname { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("fax")]
    public string? Fax { get; set; }

    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    [JsonPropertyName("web")]
    public string? Web { get; set; }

    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    [JsonPropertyName("role_ids")]
    public List<long>? RoleIds { get; set; }

    [JsonPropertyName("group_ids")]
    public Dictionary<string, List<string>>? GroupIds { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    [JsonPropertyName("verified")]
    public bool? Verified { get; set; }

    [JsonPropertyName("vip")]
    public bool? Vip { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("out_of_office")]
    public bool? OutOfOffice { get; set; }

    [JsonPropertyName("out_of_office_start_at")]
    public string? OutOfOfficeStartAt { get; set; }

    [JsonPropertyName("out_of_office_end_at")]
    public string? OutOfOfficeEndAt { get; set; }

    [JsonPropertyName("out_of_office_replacement_id")]
    public long? OutOfOfficeReplacementId { get; set; }

    [JsonPropertyName("preferences")]
    public Dictionary<string, JsonElement>? Preferences { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
