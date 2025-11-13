using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tickets;

/// <summary>
/// Represents a Zammad ticket.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/index.html">Ticket API</see>
/// </remarks>
public sealed class Ticket
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("group_id")]
    public long GroupId { get; set; }

    [JsonPropertyName("state_id")]
    public long StateId { get; set; }

    [JsonPropertyName("priority_id")]
    public long PriorityId { get; set; }

    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }

    [JsonPropertyName("customer_id")]
    public long CustomerId { get; set; }

    [JsonPropertyName("owner_id")]
    public long OwnerId { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("time_unit")]
    public string? TimeUnit { get; set; }

    [JsonPropertyName("preferences")]
    public Dictionary<string, JsonElement>? Preferences { get; set; }

    [JsonPropertyName("first_response_at")]
    public DateTimeOffset? FirstResponseAt { get; set; }

    [JsonPropertyName("first_response_escalation_at")]
    public DateTimeOffset? FirstResponseEscalationAt { get; set; }

    [JsonPropertyName("first_response_in_min")]
    public int? FirstResponseInMin { get; set; }

    [JsonPropertyName("first_response_diff_in_min")]
    public int? FirstResponseDiffInMin { get; set; }

    [JsonPropertyName("close_at")]
    public DateTimeOffset? CloseAt { get; set; }

    [JsonPropertyName("close_escalation_at")]
    public DateTimeOffset? CloseEscalationAt { get; set; }

    [JsonPropertyName("close_in_min")]
    public int? CloseInMin { get; set; }

    [JsonPropertyName("close_diff_in_min")]
    public int? CloseDiffInMin { get; set; }

    [JsonPropertyName("update_escalation_at")]
    public DateTimeOffset? UpdateEscalationAt { get; set; }

    [JsonPropertyName("update_in_min")]
    public int? UpdateInMin { get; set; }

    [JsonPropertyName("update_diff_in_min")]
    public int? UpdateDiffInMin { get; set; }

    [JsonPropertyName("last_contact_at")]
    public DateTimeOffset? LastContactAt { get; set; }

    [JsonPropertyName("last_contact_agent_at")]
    public DateTimeOffset? LastContactAgentAt { get; set; }

    [JsonPropertyName("last_contact_customer_at")]
    public DateTimeOffset? LastContactCustomerAt { get; set; }

    [JsonPropertyName("last_owner_update_at")]
    public DateTimeOffset? LastOwnerUpdateAt { get; set; }

    [JsonPropertyName("escalation_at")]
    public DateTimeOffset? EscalationAt { get; set; }

    [JsonPropertyName("pending_time")]
    public DateTimeOffset? PendingTime { get; set; }

    [JsonPropertyName("create_article_type_id")]
    public long? CreateArticleTypeId { get; set; }

    [JsonPropertyName("create_article_sender_id")]
    public long? CreateArticleSenderId { get; set; }

    [JsonPropertyName("article_count")]
    public int ArticleCount { get; set; }

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
