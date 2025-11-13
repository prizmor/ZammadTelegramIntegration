using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tickets;

/// <summary>
/// Represents a ticket article.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/articles.html">Ticket Articles API</see>
/// </remarks>
public sealed class TicketArticle
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("ticket_id")]
    public long TicketId { get; set; }

    [JsonPropertyName("type_id")]
    public long TypeId { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("sender_id")]
    public long SenderId { get; set; }

    [JsonPropertyName("sender")]
    public string? Sender { get; set; }

    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("to")]
    public string? To { get; set; }

    [JsonPropertyName("cc")]
    public string? Cc { get; set; }

    [JsonPropertyName("reply_to")]
    public string? ReplyTo { get; set; }

    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("internal")]
    public bool Internal { get; set; }

    [JsonPropertyName("preferences")]
    public Dictionary<string, JsonElement>? Preferences { get; set; }

    [JsonPropertyName("attachments")]
    public List<TicketArticleAttachment>? Attachments { get; set; }

    [JsonPropertyName("origin_by_id")]
    public long? OriginById { get; set; }

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
