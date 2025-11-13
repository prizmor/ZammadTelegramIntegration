using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Links;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Gets all links for a specific object.
    /// </summary>
    /// <param name="linkObject">The object type (e.g., "Ticket").</param>
    /// <param name="linkObjectValue">The object ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Links response with links and assets.</returns>
    /// <exception cref="ArgumentException">Thrown when linkObject is null or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when linkObjectValue is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/links.html">Ticket Links API</see>
    /// </remarks>
    public async Task<LinksResponse> GetLinksAsync(
        string linkObject,
        long linkObjectValue,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(linkObject)) throw new ArgumentException("Link object cannot be null or whitespace.", nameof(linkObject));
        if (linkObjectValue <= 0) throw new ArgumentOutOfRangeException(nameof(linkObjectValue), "Link object value must be greater than 0.");

        return await SendAsync<LinksResponse>(
            HttpMethod.Get,
            $"/api/v1/links?link_object={Uri.EscapeDataString(linkObject)}&link_object_value={linkObjectValue}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds a link between two objects.
    /// </summary>
    /// <param name="linkType">The link type (e.g., "normal").</param>
    /// <param name="sourceTicketNumber">The source ticket number.</param>
    /// <param name="targetTicketId">The target ticket ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentException">Thrown when linkType or sourceTicketNumber is null or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when targetTicketId is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/links.html">Ticket Links API</see>
    /// </remarks>
    public async Task AddLinkAsync(
        string linkType,
        string sourceTicketNumber,
        long targetTicketId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(linkType)) throw new ArgumentException("Link type cannot be null or whitespace.", nameof(linkType));
        if (string.IsNullOrWhiteSpace(sourceTicketNumber)) throw new ArgumentException("Source ticket number cannot be null or whitespace.", nameof(sourceTicketNumber));
        if (targetTicketId <= 0) throw new ArgumentOutOfRangeException(nameof(targetTicketId), "Target ticket ID must be greater than 0.");

        var request = new LinkAddRequest
        {
            LinkType = linkType,
            LinkObjectSource = "Ticket",
            LinkObjectSourceNumber = sourceTicketNumber,
            LinkObjectTarget = "Ticket",
            LinkObjectTargetValue = targetTicketId
        };

        await SendAsync<object>(HttpMethod.Post, "/api/v1/links/add", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a link between two objects.
    /// </summary>
    /// <param name="linkType">The link type (e.g., "normal").</param>
    /// <param name="sourceTicketNumber">The source ticket number.</param>
    /// <param name="targetTicketId">The target ticket ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentException">Thrown when linkType or sourceTicketNumber is null or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when targetTicketId is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/links.html">Ticket Links API</see>
    /// </remarks>
    public async Task RemoveLinkAsync(
        string linkType,
        string sourceTicketNumber,
        long targetTicketId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(linkType)) throw new ArgumentException("Link type cannot be null or whitespace.", nameof(linkType));
        if (string.IsNullOrWhiteSpace(sourceTicketNumber)) throw new ArgumentException("Source ticket number cannot be null or whitespace.", nameof(sourceTicketNumber));
        if (targetTicketId <= 0) throw new ArgumentOutOfRangeException(nameof(targetTicketId), "Target ticket ID must be greater than 0.");

        var request = new LinkRemoveRequest
        {
            LinkType = linkType,
            LinkObjectSource = "Ticket",
            LinkObjectSourceNumber = sourceTicketNumber,
            LinkObjectTarget = "Ticket",
            LinkObjectTargetValue = targetTicketId
        };

        await SendAsync<object>(HttpMethod.Delete, "/api/v1/links/remove", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
