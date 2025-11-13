using System.Text.Json.Serialization;

namespace Zammad.Sdk.Tickets;

/// <summary>
/// Represents an attachment for creating a ticket article.
/// </summary>
/// <remarks>
/// See <see href="https://docs.zammad.org/en/latest/api/ticket/articles.html">Ticket Articles API</see>
/// </remarks>
public sealed class ArticleAttachment
{
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("mime-type")]
    public string? MimeType { get; set; }
}
