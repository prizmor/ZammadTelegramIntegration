using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tickets;

/// <summary>
/// Request to create a standalone ticket article.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/articles.html">Ticket Articles API</see>
/// </remarks>
public sealed class TicketArticleStandaloneCreateRequest
{
    [JsonPropertyName("ticket_id")]
    public long TicketId { get; set; }

    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("sender")]
    public string? Sender { get; set; }

    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("internal")]
    public bool? Internal { get; set; }

    [JsonPropertyName("time_unit")]
    public string? TimeUnit { get; set; }

    [JsonPropertyName("to")]
    public string? To { get; set; }

    [JsonPropertyName("cc")]
    public string? Cc { get; set; }

    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("reply_to")]
    public string? ReplyTo { get; set; }

    [JsonPropertyName("origin_by_id")]
    public long? OriginById { get; set; }

    [JsonPropertyName("attachments")]
    public List<ArticleAttachment>? Attachments { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
