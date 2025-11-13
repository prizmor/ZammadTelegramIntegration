using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Tickets;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Gets all articles for a specific ticket.
    /// </summary>
    /// <param name="ticketId">The ticket ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of articles for the ticket.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when ticketId is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/articles.html">Ticket Articles API</see>
    /// </remarks>
    public async Task<IReadOnlyList<TicketArticle>> GetTicketArticlesAsync(
        long ticketId,
        CancellationToken cancellationToken = default)
    {
        if (ticketId <= 0) throw new ArgumentOutOfRangeException(nameof(ticketId), "Ticket ID must be greater than 0.");

        var articles = await SendAsync<List<TicketArticle>>(
            HttpMethod.Get,
            $"/api/v1/ticket_articles/by_ticket/{ticketId}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
        return articles;
    }

    /// <summary>
    /// Gets a specific article by ID.
    /// </summary>
    /// <param name="articleId">The article ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The article.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when articleId is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/articles.html">Ticket Articles API</see>
    /// </remarks>
    public async Task<TicketArticle> GetTicketArticleAsync(
        long articleId,
        CancellationToken cancellationToken = default)
    {
        if (articleId <= 0) throw new ArgumentOutOfRangeException(nameof(articleId), "Article ID must be greater than 0.");

        return await SendAsync<TicketArticle>(
            HttpMethod.Get,
            $"/api/v1/ticket_articles/{articleId}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new article for an existing ticket.
    /// </summary>
    /// <param name="request">The article creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created article.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when required fields are missing.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/articles.html">Ticket Articles API</see>
    /// </remarks>
    public async Task<TicketArticle> CreateTicketArticleAsync(
        TicketArticleStandaloneCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (request.TicketId <= 0) throw new ArgumentException("TicketId must be greater than 0.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.Body)) throw new ArgumentException("Body is required.", nameof(request));

        return await SendAsync<TicketArticle>(
            HttpMethod.Post,
            "/api/v1/ticket_articles",
            request,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads a ticket attachment as binary data.
    /// </summary>
    /// <param name="ticketId">The ticket ID.</param>
    /// <param name="articleId">The article ID.</param>
    /// <param name="attachmentId">The attachment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The attachment binary data.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any ID is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/articles.html">Ticket Articles API</see>
    /// </remarks>
    public async Task<byte[]> DownloadTicketAttachmentAsync(
        long ticketId,
        long articleId,
        long attachmentId,
        CancellationToken cancellationToken = default)
    {
        if (ticketId <= 0) throw new ArgumentOutOfRangeException(nameof(ticketId), "Ticket ID must be greater than 0.");
        if (articleId <= 0) throw new ArgumentOutOfRangeException(nameof(articleId), "Article ID must be greater than 0.");
        if (attachmentId <= 0) throw new ArgumentOutOfRangeException(nameof(attachmentId), "Attachment ID must be greater than 0.");

        return await DownloadBinaryAsync(
            $"/api/v1/ticket_attachment/{ticketId}/{articleId}/{attachmentId}",
            cancellationToken).ConfigureAwait(false);
    }
}
