using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zammad.Sdk.Tags;

namespace Zammad.Sdk;

public sealed partial class ZammadClient
{
    /// <summary>
    /// Gets all tags for a specific object.
    /// </summary>
    /// <param name="objectType">The object type (e.g., "Ticket").</param>
    /// <param name="objectId">The object ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of tag names.</returns>
    /// <exception cref="ArgumentException">Thrown when objectType is null or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when objectId is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
    /// </remarks>
    public async Task<IReadOnlyList<string>> GetTagsAsync(
        string objectType,
        long objectId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectType)) throw new ArgumentException("Object type cannot be null or whitespace.", nameof(objectType));
        if (objectId <= 0) throw new ArgumentOutOfRangeException(nameof(objectId), "Object ID must be greater than 0.");

        var tags = await SendAsync<List<string>>(
            HttpMethod.Get,
            $"/api/v1/tags?object={Uri.EscapeDataString(objectType)}&o_id={objectId}",
            cancellationToken: cancellationToken).ConfigureAwait(false);
        return tags;
    }

    /// <summary>
    /// Adds a tag to an object.
    /// </summary>
    /// <param name="item">The tag name to add.</param>
    /// <param name="objectType">The object type (e.g., "Ticket").</param>
    /// <param name="objectId">The object ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentException">Thrown when item or objectType is null or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when objectId is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
    /// </remarks>
    public async Task AddTagAsync(
        string item,
        string objectType,
        long objectId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(item)) throw new ArgumentException("Item cannot be null or whitespace.", nameof(item));
        if (string.IsNullOrWhiteSpace(objectType)) throw new ArgumentException("Object type cannot be null or whitespace.", nameof(objectType));
        if (objectId <= 0) throw new ArgumentOutOfRangeException(nameof(objectId), "Object ID must be greater than 0.");

        var request = new TagAddRequest
        {
            Item = item,
            Object = objectType,
            ObjectId = objectId
        };

        await SendAsync<object>(HttpMethod.Post, "/api/v1/tags/add", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a tag from an object.
    /// </summary>
    /// <param name="item">The tag name to remove.</param>
    /// <param name="objectType">The object type (e.g., "Ticket").</param>
    /// <param name="objectId">The object ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentException">Thrown when item or objectType is null or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when objectId is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
    /// </remarks>
    public async Task RemoveTagAsync(
        string item,
        string objectType,
        long objectId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(item)) throw new ArgumentException("Item cannot be null or whitespace.", nameof(item));
        if (string.IsNullOrWhiteSpace(objectType)) throw new ArgumentException("Object type cannot be null or whitespace.", nameof(objectType));
        if (objectId <= 0) throw new ArgumentOutOfRangeException(nameof(objectId), "Object ID must be greater than 0.");

        var request = new TagRemoveRequest
        {
            Item = item,
            Object = objectType,
            ObjectId = objectId
        };

        await SendAsync<object>(HttpMethod.Delete, "/api/v1/tags/remove", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Lists all tags in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of tags.</returns>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
    /// </remarks>
    public async Task<IReadOnlyList<Tag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        var tags = await SendAsync<List<Tag>>(HttpMethod.Get, "/api/v1/tag_list", cancellationToken: cancellationToken).ConfigureAwait(false);
        return tags;
    }

    /// <summary>
    /// Creates a new tag in the system.
    /// </summary>
    /// <param name="name">The tag name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created tag.</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or whitespace.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
    /// </remarks>
    public async Task<Tag> CreateTagAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        var request = new TagCreateRequest { Name = name };
        return await SendAsync<Tag>(HttpMethod.Post, "/api/v1/tag_list", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates (renames) a tag.
    /// </summary>
    /// <param name="id">The tag ID.</param>
    /// <param name="name">The new tag name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated tag.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <exception cref="ArgumentException">Thrown when name is null or whitespace.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
    /// </remarks>
    public async Task<Tag> UpdateTagAsync(long id, string name, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Tag ID must be greater than 0.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        var request = new TagUpdateRequest { Name = name };
        return await SendAsync<Tag>(HttpMethod.Put, $"/api/v1/tag_list/{id}", request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a tag from the system.
    /// </summary>
    /// <param name="id">The tag ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when id is less than or equal to 0.</exception>
    /// <remarks>
    /// See <see href="https://docs.zammad.org/en/latest/api/ticket/tags.html">Tags API</see>
    /// </remarks>
    public async Task DeleteTagAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Tag ID must be greater than 0.");

        await SendAsync<object>(HttpMethod.Delete, $"/api/v1/tag_list/{id}", cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
